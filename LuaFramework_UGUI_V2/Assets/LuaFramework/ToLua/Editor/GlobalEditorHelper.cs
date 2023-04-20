#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using System.Diagnostics;

public static class GlobalEditorHelper
{
	/// <summary>
	/// 将路径中的\全部换为/,如果以/结尾,则移除最后一个/
	/// </summary>
	/// <returns></returns>
	public static string RepairPath(string path)
	{
		string result = path.Replace('\\', '/');
		return result.TrimEnd('/');
	}

	public static string GetAssetsPath(string oldPath)
	{
		oldPath = RepairPath(oldPath);
		return oldPath.Substring(oldPath.IndexOf("Assets"));
	}

	public static string GetResourcePath(string assetPath)
	{
		assetPath = RepairPath(assetPath);
		assetPath = assetPath.Substring(assetPath.IndexOf("/Resources") + "/Resources/".Length);
		int pos = assetPath.LastIndexOf("."); // 有可能没有扩展名
		if (pos < 0)
		{
			pos = assetPath.Length;
		}
		return assetPath.Substring(0, pos);
	}
	
	public static void DelFolder(string path, bool IncludeRoot = true)
	{
		path = path.Replace('\\', '/');
		if (!Directory.Exists(path))
		{
			return;
		}
		var files = Directory.GetFiles(path, @"*.*", SearchOption.TopDirectoryOnly);
		foreach (var file in files)
		{
			DelFile(file);
		}
		var dirs = Directory.GetDirectories(path, @"*.*", SearchOption.TopDirectoryOnly);
		for (int i = 0; i < dirs.Length; i++)
		{
			var dir = dirs[i];
			if (!dir.Contains("$") && !dir.Contains("Boot"))
			{
				DelFolder(dir);
			}
		}
		if(IncludeRoot)
		{
			Directory.Delete(path, true);
		}
	}

	public static void DelFile(string file)
	{
		if ((File.GetAttributes(file) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
		{
			File.SetAttributes(file, FileAttributes.Normal);
		}
		File.Delete(file);
	}

	public static void EditFile(string file)
	{
		if (File.GetAttributes(file).ToString().IndexOf("ReadOnly") != -1)
		{
			File.SetAttributes(file, FileAttributes.Normal);
		}
	}

	public static bool CopyFile(string src, string dest, bool force = true)
	{
		if (File.Exists(dest) && ((File.GetAttributes(dest) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly))
		{
			File.SetAttributes(dest, FileAttributes.Normal);
		}
		if (!force && File.Exists(dest))
		{
			return false;
		}
		File.Copy(src, dest, force);
		return true;
	}

	/// <summary>
	/// 根据扩展名获取文件路径(包括文件名和扩展名)
	/// </summary>
	/// <param name="dirPath">文件夹路径</param>
	/// <param name="extension">文件扩展名</param>
	/// <param name="isDeepSearch">是否检索所有层级目录</param>
	/// <returns>返回的路径是从Assets开始的</returns>
	public static List<string> GetFilPathsByExtensions(string dirPath, string extension, bool isDeepSearch = true)
	{
		if (!Directory.Exists(dirPath))
		{
			return null;
		}
		return Directory.GetFiles(dirPath, extension, isDeepSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select(t => RepairPath(t)).ToList<string>();
	}

	/// <summary>
	/// 根据扩展名获取文件路径(包括文件名和扩展名)
	/// </summary>
	/// <param name="dirPaths">文件夹路径</param>
	/// <param name="extension">文件扩展名</param>
	/// <param name="isDeepSearch">是否检索所有层级目录</param>
	/// <returns>返回的路径是从Assets开始的</returns>
	public static List<string> GetFilPathsByExtensions(string[] dirPaths, string extension, bool isDeepSearch = true)
	{
		List<string> allFiles = new List<string>();
		foreach (var dirPath in dirPaths)
		{
			if (!Directory.Exists(dirPath))
			{
				continue;
			}
			var files = GetFilPathsByExtensions(dirPath, extension);
			allFiles.InsertRange(allFiles.Count, files);
		}
		return allFiles;
	}

	/// <summary>
	/// 根据扩展名获取文件路径(包括文件名和扩展名)
	/// </summary>
	/// <param name="dirPath">文件夹路径</param>
	/// <param name="extensions">文件扩展名</param>
	/// <param name="isDeepSearch">是否检索所有层级目录</param>
	/// <returns>返回的路径是从Assets开始的</returns>
	public static List<string> GetFilPathsByExtensions(string dirPath, string[] extensions, bool isDeepSearch = true)
	{
		if (!Directory.Exists(dirPath))
		{
			return null;
		}
		List<string> allFiles = new List<string>();
		foreach (var extension in extensions)
		{
			var files = GetFilPathsByExtensions(dirPath, extension, isDeepSearch);
			allFiles.InsertRange(allFiles.Count, files);
		}
		return allFiles;
	}

	/// <summary>
	/// 根据扩展名获取文件路径(包括文件名和扩展名)
	/// </summary>
	/// <param name="dirPaths">文件夹路径</param>
	/// <param name="extensions">文件扩展名</param>
	/// <param name="isDeepSearch">是否检索所有层级目录</param>
	/// <returns>返回的路径是从Assets开始的</returns>
	public static List<string> GetFilPathsByExtensions(string[] dirPaths, string[] extensions, bool isDeepSearch = true)
	{
		List<string> allFiles = new List<string>();
		foreach (var dirPath in dirPaths)
		{
			if (!Directory.Exists(dirPath))
			{
				continue;
			}
			foreach (var extension in extensions)
			{
				var files = GetFilPathsByExtensions(dirPath, extension);
				allFiles.InsertRange(allFiles.Count, files);
			}
		}
		return allFiles;
	}

	/// <summary>
	/// 根据扩展名获取以Assets目录开始的文件路径(包括文件名和扩展名)
	/// </summary>
	/// <param name="dirPath">文件夹路径</param>
	/// <param name="extension">文件扩展名</param>
	/// <param name="isDeepSearch">是否检索所有层级目录</param>
	/// <returns>返回的路径是从Assets开始的</returns>
	public static List<string> GetFileAssetsPathsByExtensions(string dirPath, string extension, bool isDeepSearch = true)
	{
		if (!Directory.Exists(dirPath))
		{
			return null;
		}
		return Directory.GetFiles(dirPath, extension, isDeepSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select(t => GetAssetsPath(t)).ToList<string>();
	}

	/// <summary>
	/// 根据扩展名获取以Assets目录开始的文件路径(包括文件名和扩展名)
	/// </summary>
	/// <param name="dirPaths">文件夹路径</param>
	/// <param name="extension">文件扩展名</param>
	/// <param name="isDeepSearch">是否检索所有层级目录</param>
	/// <returns>返回的路径是从Assets开始的</returns>
	public static List<string> GetFileAssetsPathsByExtensions(string[] dirPaths, string extension, bool isDeepSearch = true)
	{
		List<string> allFiles = new List<string>();
		foreach (var dirPath in dirPaths)
		{
			var files = GetFileAssetsPathsByExtensions(dirPath, extension, isDeepSearch);
			if (files != null)
			{
				allFiles.InsertRange(allFiles.Count, files);
			}
		}
		return allFiles;
	}

	/// <summary>
	/// 根据扩展名获取以Assets目录开始的文件路径(包括文件名和扩展名)
	/// </summary>
	/// <param name="dirPath">文件夹路径</param>
	/// <param name="extensions">文件扩展名</param>
	/// <param name="isDeepSearch">是否检索所有层级目录</param>
	/// <returns>返回的路径是从Assets开始的</returns>
	public static List<string> GetFileAssetsPathsByExtensions(string dirPath, string[] extensions, bool isDeepSearch = true)
	{
		List<string> allFiles = new List<string>();
		foreach (var extension in extensions)
		{
			var files = GetFileAssetsPathsByExtensions(dirPath, extension, isDeepSearch);
			if (files != null)
			{
				allFiles.InsertRange(allFiles.Count, files);
			}
		}
		return allFiles;
	}

	/// <summary>
	/// 根据扩展名获取以Assets目录开始的文件路径(包括文件名和扩展名)
	/// </summary>
	/// <param name="dirPaths">文件夹路径</param>
	/// <param name="extensions">文件扩展名</param>
	/// <param name="isDeepSearch">是否检索所有层级目录</param>
	/// <returns>返回的路径是从Assets开始的</returns>
	public static List<string> GetFileAssetsPathsByExtensions(string[] dirPaths, string[] extensions, bool isDeepSearch = true)
	{
		List<string> allFiles = new List<string>();
		foreach (var dirPath in dirPaths)
		{
			foreach (var extension in extensions)
			{
				var files = GetFileAssetsPathsByExtensions(dirPath, extension, isDeepSearch);
				if (files != null)
				{
					allFiles.InsertRange(allFiles.Count, files);
				}
			}
		}
		return allFiles;
	}

	/// <summary>
	/// 根据扩展名获取Resources以下的文件路径(包括文件名和扩展名)
	/// </summary>
	/// <param name="dirPath">文件夹路径</param>
	/// <param name="extension">文件扩展名</param>
	/// <param name="isDeepSearch">是否检索所有层级目录</param>
	/// <returns>返回的路径是从Assets开始的</returns>
	public static List<string> GetFileResourcesPathsByExtensions(string dirPath, string extension, bool isDeepSearch = true)
	{
		if (!Directory.Exists(dirPath))
		{
			return null;
		}
		return Directory.GetFiles(dirPath, extension, isDeepSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select(t => GetResourcePath(t)).ToList<string>();
	}

	/// <summary>
	/// 根据扩展名获取Resources以下的文件路径(包括文件名和扩展名)
	/// </summary>
	/// <param name="dirPaths">文件夹路径</param>
	/// <param name="extension">文件扩展名</param>
	/// <param name="isDeepSearch">是否检索所有层级目录</param>
	/// <returns>返回的路径是Resources/后的</returns>
	public static List<string> GetFileResourcesPathsByExtensions(string[] dirPaths, string extension, bool isDeepSearch = true)
	{
		List<string> allFiles = new List<string>();
		foreach (var dirPath in dirPaths)
		{
			if (!Directory.Exists(dirPath))
			{
				continue;
			}
			var files = GetFileResourcesPathsByExtensions(dirPath, extension, isDeepSearch);
			allFiles.InsertRange(allFiles.Count, files);
		}
		return allFiles;
	}

	/// <summary>
	/// 根据扩展名获取Resources以下的文件路径(包括文件名和扩展名)
	/// </summary>
	/// <param name="dirPath">文件夹路径</param>
	/// <param name="extensions">文件扩展名</param>
	/// <param name="isDeepSearch">是否检索所有层级目录</param>
	/// <returns>返回的路径是从Assets开始的</returns>
	public static List<string> GetFileResourcesPathsByExtensions(string dirPath, string[] extensions, bool isDeepSearch = true)
	{
		if (!Directory.Exists(dirPath))
		{
			return null;
		}
		List<string> allFiles = new List<string>();
		foreach (var extension in extensions)
		{
			var files = GetFileResourcesPathsByExtensions(dirPath, extension, isDeepSearch);
			allFiles.InsertRange(allFiles.Count, files);
		}
		return allFiles;
	}

	/// <summary>
	/// 根据扩展名获取Resources以下的文件路径(包括文件名和扩展名)
	/// </summary>
	/// <param name="dirPaths">文件夹路径</param>
	/// <param name="extensions">文件扩展名</param>
	/// <param name="isDeepSearch">是否检索所有层级目录</param>
	/// <returns>返回的路径是从Assets开始的</returns>
	public static List<string> GetFileResourcesPathsByExtensions(string[] dirPaths, string[] extensions, bool isDeepSearch = true)
	{
		List<string> allFiles = new List<string>();
		foreach (var dirPath in dirPaths)
		{
			if (!Directory.Exists(dirPath))
			{
				continue;
			}
			foreach (var extension in extensions)
			{

				var files = GetFileResourcesPathsByExtensions(dirPath, extension, isDeepSearch);
				allFiles.InsertRange(allFiles.Count, files);
			}
		}
		return allFiles;
	}

	public static string ExecuteCmdNoWait(string wrokDirectory, string dosCommand, string args = null)
	{
		string output = string.Empty;

		if (string.IsNullOrEmpty(wrokDirectory) && string.IsNullOrEmpty(dosCommand))
		{
			output += "Error: 缺少参数！";
			return output;
		}

		Process process = new Process();
		ProcessStartInfo startInfo = new ProcessStartInfo();
		startInfo.FileName = dosCommand;
		startInfo.UseShellExecute = false;
		startInfo.RedirectStandardInput = true;
		startInfo.RedirectStandardOutput = true;
		startInfo.RedirectStandardError = true;
		startInfo.CreateNoWindow = true;
		if (args != null)
		{
			startInfo.Arguments = args;
		}
		startInfo.WorkingDirectory = wrokDirectory;
		process.StartInfo = startInfo;

		try
		{
			if (process.Start())       //开始进程
			{
				output = process.StandardOutput.ReadToEnd();     //读取输出流释放缓冲
				output += Environment.NewLine + process.StandardError.ReadToEnd();

				process.Close();
				//LogWrapper.LogDebug(dosCommand + " run finish !!! output " + output);
			}
		}
		catch (Exception e)
		{
			process.Close();
			LogWrapper.LogError(e.Message,"\n",e.StackTrace);

			output += $"Exception: 发生异常 {e.Message}";
		}

		return output;
	}
	public static void ExecuteShellCmd(string wrokDirectory, string dosCommand, string args = null)
	{
		Process p = null;
		try
		{
			var pStartInfo = new System.Diagnostics.ProcessStartInfo(dosCommand);
			pStartInfo.Arguments = args;
			pStartInfo.CreateNoWindow = false;
			pStartInfo.UseShellExecute = true;
			pStartInfo.RedirectStandardError = false;
			pStartInfo.RedirectStandardInput = false;
			pStartInfo.RedirectStandardOutput = false;
			if (!string.IsNullOrEmpty(wrokDirectory))
				pStartInfo.WorkingDirectory = wrokDirectory;

			p = System.Diagnostics.Process.Start(pStartInfo);
		}
		catch(Exception e)
		{
			LogWrapper.LogError(e.Message, "\n", e.StackTrace);
		}
		finally
		{
			p.Close();
		}
	}
	public static bool EnableScriptingDefineSymbols(BuildTargetGroup buildTargetGroup, string ds, bool flag)
	{
		string existSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

		if (flag)
		{
			if (!existSymbols.Contains(ds))
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, existSymbols + ";" + ds);
				return true;
			}
		}
		else
		{
			if (existSymbols.Contains(ds))
			{
				var newSymbols = existSymbols.Replace(";" + ds, "");
				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newSymbols);
				return true;
			}
		}
		return false;
	}
}
#endif