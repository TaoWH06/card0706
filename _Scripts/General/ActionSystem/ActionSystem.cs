using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null;
    public bool IsPerforming { get; private set; } = false;
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();
    public void Perform(GameAction action, System.Action OnPerformFinished = null)// OnPerformFinished = null：可选回调，整套动作全部跑完后触发，外部可传逻辑（刷新 UI、开启下一轮、结算奖励）
    {
        if (IsPerforming) return;
        IsPerforming = true;
        StartCoroutine(Flow(action, () =>
        {
            IsPerforming = false;
            OnPerformFinished?.Invoke();
        }));
    }

    public void AddReaction(GameAction gameAction)
    {
        reactions?.Add(gameAction);
    }

    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        reactions = action.PreReactions;
        //Debug.Log($"执行: {action.GetType().Name}；PER列表为\n" + ListToString(reactions));
        PerformSubscribers(action, preSubs);// PerformSubscribers：同步、即时、数值 / 逻辑判断，一帧跑完；
        yield return PerformReactions();// PerformReactions：异步、带延时、视觉动画，协程串行等待；
        // Debug.Log($"执行PRE后: {action.GetType().Name}；PER列表为\n" + ListToString(reactions));

        reactions = action.PerformReactions;
        // Debug.Log($"执行: {action.GetType().Name}；PERform列表为\n" + ListToString(reactions));
        yield return PerformPerformer(action);
        yield return PerformReactions();
        // Debug.Log($"执行PERform后: {action.GetType().Name}；PERform列表为\n" + ListToString(reactions));

        reactions = action.PostReactions;
        // Debug.Log($"执行: {action.GetType().Name}；POST列表为\n" + ListToString(reactions));
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();
        // Debug.Log($"执行POST后: {action.GetType().Name}；POST列表为\n" + ListToString(reactions));

        OnFlowFinished?.Invoke();
    }

    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            yield return performers[type](action);
        }
    }

    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = action.GetType();
        if (subs.ContainsKey(type))
        {
            foreach (var sub in subs[type])
            {
                sub(action);
            }
        }
    }

    private IEnumerator PerformReactions()
    {
        foreach (var reaction in reactions)
        {
            // Debug.Log($"执行: {reaction.GetType().Name}；列表为\n" + ListToString(reactions));
            yield return Flow(reaction);// Flow 会修改 reactions!??
        }
    }

    /// <summary>打印集合所有元素，自动拼接换行</summary>
    public static string ListToString<T>(List<T> list)
    {
        if (list == null) return "List is null";
        if (list.Count == 0) return "List empty";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"List<{typeof(T).Name}> Count = {list.Count}");
        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine($"[{i}] {list[i]}");
        }
        return sb.ToString();
    }

    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Type type = typeof(T);
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);// 字典统一存储 Func<GameAction, IEnumerator>（入参是基类），但外部传入的委托只接收子类 T，所以做一层封装
        if (performers.ContainsKey(type)) performers[type] = wrappedPerformer;
        else performers.Add(type, wrappedPerformer);
    }
    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type)) performers.Remove(type);
    }

    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        void wrappedReaction(GameAction action) => reaction((T)action);//容器字典只能存储 Action<GameAction>（基类参数）；外部传入的回调只接收 T 子类；内层包装函数接收基类，强制转型为 T 交给外部回调
        if (subs.ContainsKey(typeof(T)))
        {
            subs[typeof(T)].Add(wrappedReaction);
        }
        else
        {
            subs.Add(typeof(T), new());
            subs[typeof(T)].Add(wrappedReaction);
        }
    }

    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(GameAction action) => reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }
}
