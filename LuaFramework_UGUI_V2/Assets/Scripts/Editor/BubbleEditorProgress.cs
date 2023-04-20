using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class BubbleEditorProgress
{
    private static Progress p;
    
    public class Progress : IDisposable
    {
        //释放进程
        public void Dispose()
        {
            EditorUtility.ClearProgressBar();
            EndProgress();
        }

        public Progress(string title , int cur)
        {
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayCancelableProgressBar(title,$"当前总数为:{cur}" , 0f);
            }
        }

        
    }
    

    public static Progress CreateProgress(string title ,int cur)
    {
        if (Application.isBatchMode)
        {
            return null;
        }
        p = new Progress(title, cur);
        return p;
    }

    public static void EndProgress()
    {
        p = null;
    }

    public static bool Display(string title , int cur , int total)
    {
        if (p == null) return false;
        if (!Application.isBatchMode)
        {
            EditorUtility.DisplayCancelableProgressBar(title,$"当前进度为:{cur} / {total}" , 0f);
            return true;
        }
        return false;
    }
}
