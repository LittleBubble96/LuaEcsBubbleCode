using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Bubble.Tools
{
    public static class ReflectionTools 
    {
        /// <summary>
        /// 实例化所有子例集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetInstancesOfSubClass<T>()
        {
            //获得程序集中的所有类型
            var types = Assembly.GetCallingAssembly().GetTypes();
            var instanceAbsType = typeof(T);
            var instances = new List<T>();
            foreach (var type in types)
            {
                var baseType = type.BaseType;
                while (baseType!=null)
                {
                    //如果基类相等，实例化此对象
                    if (baseType.Name == instanceAbsType.Name)
                    {
                        var instance = Activator.CreateInstance(type);
                        if (instance is T inst)
                        {
                            instances.Add(inst);
                        }
                        break;
                    }
                    else
                    {
                        baseType = baseType.BaseType;
                    }
                }
            }
            return instances;
        }
    }
}
