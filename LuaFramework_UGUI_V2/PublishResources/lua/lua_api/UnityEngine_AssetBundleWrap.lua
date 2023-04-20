---@class UnityEngine.AssetBundle : UnityEngine.Object
---@field isStreamedSceneAssetBundle bool
---@field memoryBudgetKB uint
local m = {}
---@param unloadAllObjects bool
function m.UnloadAllAssetBundles(unloadAllObjects) end
---@return System.Collections.Generic.IEnumerable
function m.GetAllLoadedAssetBundles() end
---@overload fun(path:string, crc:uint):UnityEngine.AssetBundleCreateRequest
---@overload fun(path:string, crc:uint, offset:ulong):UnityEngine.AssetBundleCreateRequest
---@param path string
---@return UnityEngine.AssetBundleCreateRequest
function m.LoadFromFileAsync(path) end
---@overload fun(path:string, crc:uint):UnityEngine.AssetBundle
---@overload fun(path:string, crc:uint, offset:ulong):UnityEngine.AssetBundle
---@param path string
---@return UnityEngine.AssetBundle
function m.LoadFromFile(path) end
---@overload fun(binary:table, crc:uint):UnityEngine.AssetBundleCreateRequest
---@param binary table
---@return UnityEngine.AssetBundleCreateRequest
function m.LoadFromMemoryAsync(binary) end
---@overload fun(binary:table, crc:uint):UnityEngine.AssetBundle
---@param binary table
---@return UnityEngine.AssetBundle
function m.LoadFromMemory(binary) end
---@overload fun(stream:System.IO.Stream, crc:uint):UnityEngine.AssetBundleCreateRequest
---@overload fun(stream:System.IO.Stream):UnityEngine.AssetBundleCreateRequest
---@param stream System.IO.Stream
---@param crc uint
---@param managedReadBufferSize uint
---@return UnityEngine.AssetBundleCreateRequest
function m.LoadFromStreamAsync(stream, crc, managedReadBufferSize) end
---@overload fun(stream:System.IO.Stream, crc:uint):UnityEngine.AssetBundle
---@overload fun(stream:System.IO.Stream):UnityEngine.AssetBundle
---@param stream System.IO.Stream
---@param crc uint
---@param managedReadBufferSize uint
---@return UnityEngine.AssetBundle
function m.LoadFromStream(stream, crc, managedReadBufferSize) end
---@param password string
function m.SetAssetBundleDecryptKey(password) end
---@param name string
---@return bool
function m:Contains(name) end
---@overload fun(name:string, type:System.Type):UnityEngine.Object
---@param name string
---@return UnityEngine.Object
function m:LoadAsset(name) end
---@overload fun(name:string, type:System.Type):UnityEngine.AssetBundleRequest
---@param name string
---@return UnityEngine.AssetBundleRequest
function m:LoadAssetAsync(name) end
---@overload fun(name:string, type:System.Type):table
---@param name string
---@return table
function m:LoadAssetWithSubAssets(name) end
---@overload fun(name:string, type:System.Type):UnityEngine.AssetBundleRequest
---@param name string
---@return UnityEngine.AssetBundleRequest
function m:LoadAssetWithSubAssetsAsync(name) end
---@overload fun(type:System.Type):table
---@return table
function m:LoadAllAssets() end
---@overload fun(type:System.Type):UnityEngine.AssetBundleRequest
---@return UnityEngine.AssetBundleRequest
function m:LoadAllAssetsAsync() end
---@param unloadAllLoadedObjects bool
function m:Unload(unloadAllLoadedObjects) end
---@param unloadAllLoadedObjects bool
---@return UnityEngine.AsyncOperation
function m:UnloadAsync(unloadAllLoadedObjects) end
---@return table
function m:GetAllAssetNames() end
---@return table
function m:GetAllScenePaths() end
---@param inputPath string
---@param outputPath string
---@param method UnityEngine.BuildCompression
---@param expectedCRC uint
---@param priority UnityEngine.ThreadPriority
---@return UnityEngine.AssetBundleRecompressOperation
function m.RecompressAssetBundleAsync(inputPath, outputPath, method, expectedCRC, priority) end
UnityEngine = {}
UnityEngine.AssetBundle = m
return m