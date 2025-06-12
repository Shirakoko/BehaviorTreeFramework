using UnityEngine;
using System.Collections.Generic;
using System;

public static class GhostSimulator
{
    // 是否处于能量豆状态
    public static Func<bool> IsPowerModeActive()
    {
        return () => GameManager.Instance.IsPowerModeActive();
    }

    // 玩家是否在范围内
    public static Func<bool> IsPlayerInRange(Transform ghost, float range)
    {
        return () =>
        {
            if (GameManager.Instance.PlayerTransform == null) return false;
            float distance = Vector2.Distance(ghost.position, GameManager.Instance.PlayerTransform.position);
            return distance <= range;
        };
    }

    // 巡逻
    public static Func<NodeStatus> GhostPatrol(Transform ghost, List<Vector2> points, float speed, float waitTime)
    {
        // 状态变量
        int currentIndex = 0;
        float waitTimer = 0f;
        bool isWaiting = false;
        GhostController ghostController = ghost.GetComponent<GhostController>();

        return () =>
        {
            if (points == null || points.Count == 0) {
                return NodeStatus.Failure;
            }

            if (isWaiting)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitTime)
                {
                    isWaiting = false;
                    waitTimer = 0f;
                    currentIndex = (currentIndex + 1) % points.Count;
                }
                return NodeStatus.Running;
            }

            Vector2 targetPoint = points[currentIndex];
            Vector2 currentPos = ghost.position;
            
            // 计算并设置移动速度和方向
            Vector2 direction = (targetPoint - currentPos).normalized;
            ghostController.CurrentDirection = direction;
            ghostController.CurrentVelocity = speed;
            
            // 移动幽灵
            ghost.position = Vector2.MoveTowards(
                currentPos,
                targetPoint,
                speed * Time.deltaTime
            );

            // 检查是否到达目标点
            if (Vector2.Distance(currentPos, targetPoint) < 0.1f)
            {
                isWaiting = true;
            }

            return NodeStatus.Running;
        };
    }

    // 追击
    public static Func<NodeStatus> GhostChase(Transform ghost, float speed, float range)
    {
        GhostController ghostController = ghost.GetComponent<GhostController>();

        return () =>
        {
            if (GameManager.Instance.PlayerTransform == null)
            {
                return NodeStatus.Failure;
            }

            float distanceToPlayer = Vector2.Distance(ghost.position, GameManager.Instance.PlayerTransform.position);
            
            if (distanceToPlayer <= range)
            {
                // 计算并设置移动速度和方向
                Vector2 direction = (GameManager.Instance.PlayerTransform.position - ghost.position).normalized;
                ghostController.CurrentDirection = direction;
                ghostController.CurrentVelocity = speed;

                // 移动向玩家
                ghost.position = Vector2.MoveTowards(
                    ghost.position,
                    GameManager.Instance.PlayerTransform.position,
                    speed * Time.deltaTime
                );
                return NodeStatus.Running;
            }

            return NodeStatus.Failure;
        };
    }

    // 逃跑
    public static Func<NodeStatus> GhostFlee(Transform ghost, float speed, float safeDistance)
    {
        GhostController ghostController = ghost.GetComponent<GhostController>();

        return () =>
        {
            if (GameManager.Instance.PlayerTransform == null)
            {
                return NodeStatus.Failure;
            }

            float distanceToPlayer = Vector2.Distance(ghost.position, GameManager.Instance.PlayerTransform.position);
            
            // 如果已经达到安全距离，直接返回成功
            if (distanceToPlayer >= safeDistance)
            {
                return NodeStatus.Success;
            }

            // 计算并设置移动速度和方向
            Vector2 fleeDirection = ((Vector2)ghost.position - (Vector2)GameManager.Instance.PlayerTransform.position).normalized;
            ghostController.CurrentDirection = fleeDirection;
            ghostController.CurrentVelocity = speed;

            // 移动远离玩家
            ghost.position = Vector2.MoveTowards(
                (Vector2)ghost.position,
                (Vector2)ghost.position + fleeDirection * speed * Time.deltaTime,
                speed * Time.deltaTime
            );

            return NodeStatus.Running;
        };
    }
} 