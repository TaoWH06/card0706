using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        // StartCoroutine(TestAction(10, 1.0f));
        // StartCoroutine(TestAction2(10, 0.5f));
        // StartCoroutine(MainFlow());
    }

    void Update()
    {

    }

    IEnumerator TestAction(int max, float delay)
    {
        for (int i = 0; i < max; i++)
        {
            Debug.Log("生成数字：" + i);
            yield return new WaitForSeconds(delay);
        }
        Debug.Log("结束");
    }

    IEnumerator TestAction2(int max, float delay)
    {
        for (int i = 0; i < max; i++)
        {
            yield return new WaitForSeconds(i * delay);
            Debug.Log("生成数字：" + i);
        }
        Debug.Log("结束");
    }



    // 模拟ActionSystem全局共享临时列表（根源缺陷）
    private List<string> globalSharedList = new List<string>();

    // 顶层主流水线，对应Flow(主动作)
    IEnumerator MainFlow()
    {
        // 模拟：主动作Perform阶段绑定3个子动作
        globalSharedList.Clear();
        globalSharedList.Add("GA_扣蓝");
        globalSharedList.Add("GA_单体伤害");
        globalSharedList.Add("GA_全体伤害");

        Debug.Log("【顶层】开始遍历全局列表，列表内容：" + ListToString(globalSharedList));
        yield return RunReactions();
        Debug.Log("【顶层】全部遍历完成");
    }

    // 对应原PerformReactions：遍历全局共享列表，递归执行子Flow
    IEnumerator RunReactions()
    {
        // 关键坑：直接遍历全局共享引用
        foreach (string gaName in globalSharedList)
        {
            Debug.Log($"===== 开始执行子动作：{gaName}");
            yield return SubFlow(gaName);
            Debug.Log($"===== 子动作 {gaName} 执行完毕\n");
        }
    }

    // 子动作流水线，对应子GameAction的Flow，会覆盖全局列表
    IEnumerator SubFlow(string subGaName)
    {
        Debug.Log($"【子Flow {subGaName}】执行中，直接覆盖全局sharedList");
        // 致命：子流水线直接修改全局共享列表引用，破坏上层foreach数据源
        globalSharedList = new List<string>() { "子动作内置特效A", "子动作内置特效B" };

        // 模拟子动作内部时序延时
        yield return new WaitForSeconds(0.2f);
        Debug.Log($"【子Flow {subGaName}】执行结束");
    }

    // 辅助打印列表
    string ListToString(List<string> list)
    {
        if (list == null || list.Count == 0) return "空列表";
        string s = "";
        for (int i = 0; i < list.Count; i++)
            s += $"[{i}]{list[i]} ";
        return s;
    }
}
