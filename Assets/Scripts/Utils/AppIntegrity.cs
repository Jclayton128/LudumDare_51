using System.Diagnostics;
using UnityEngine;

public static class AppIntegrity
{
    public static void Assert(bool someCondition, string message = "Assertion Failed.")
    {
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)        
        if (!someCondition)
        {
            // get the stack frame one level up (e.g. the scope that called this method)
            StackTrace stackTrace = new StackTrace(true);
            StackFrame frame = stackTrace.GetFrame(1);
            string methodName = frame.GetMethod().Name;
            string fileName = frame.GetFileName();
            throw new UnityException($"{message}\r\nFILE={fileName}\r\nMETHOD={methodName}");
        };
#endif
    }
}
