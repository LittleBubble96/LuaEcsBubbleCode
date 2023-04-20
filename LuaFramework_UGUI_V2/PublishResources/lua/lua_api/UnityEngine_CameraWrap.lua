---@class UnityEngine.Camera : UnityEngine.Behaviour
---@field nearClipPlane float
---@field farClipPlane float
---@field fieldOfView float
---@field renderingPath UnityEngine.RenderingPath
---@field actualRenderingPath UnityEngine.RenderingPath
---@field allowHDR bool
---@field allowMSAA bool
---@field allowDynamicResolution bool
---@field forceIntoRenderTexture bool
---@field orthographicSize float
---@field orthographic bool
---@field opaqueSortMode UnityEngine.Rendering.OpaqueSortMode
---@field transparencySortMode UnityEngine.TransparencySortMode
---@field transparencySortAxis UnityEngine.Vector3
---@field depth float
---@field aspect float
---@field velocity UnityEngine.Vector3
---@field cullingMask int
---@field eventMask int
---@field layerCullSpherical bool
---@field cameraType UnityEngine.CameraType
---@field overrideSceneCullingMask ulong
---@field layerCullDistances table
---@field useOcclusionCulling bool
---@field cullingMatrix UnityEngine.Matrix4x4
---@field backgroundColor UnityEngine.Color
---@field clearFlags UnityEngine.CameraClearFlags
---@field depthTextureMode UnityEngine.DepthTextureMode
---@field clearStencilAfterLightingPass bool
---@field usePhysicalProperties bool
---@field sensorSize UnityEngine.Vector2
---@field lensShift UnityEngine.Vector2
---@field focalLength float
---@field gateFit UnityEngine.Camera.GateFitMode
---@field rect UnityEngine.Rect
---@field pixelRect UnityEngine.Rect
---@field pixelWidth int
---@field pixelHeight int
---@field scaledPixelWidth int
---@field scaledPixelHeight int
---@field targetTexture UnityEngine.RenderTexture
---@field activeTexture UnityEngine.RenderTexture
---@field targetDisplay int
---@field cameraToWorldMatrix UnityEngine.Matrix4x4
---@field worldToCameraMatrix UnityEngine.Matrix4x4
---@field projectionMatrix UnityEngine.Matrix4x4
---@field nonJitteredProjectionMatrix UnityEngine.Matrix4x4
---@field useJitteredProjectionMatrixForTransparentRendering bool
---@field previousViewProjectionMatrix UnityEngine.Matrix4x4
---@field main UnityEngine.Camera
---@field current UnityEngine.Camera
---@field scene UnityEngine.SceneManagement.Scene
---@field stereoEnabled bool
---@field stereoSeparation float
---@field stereoConvergence float
---@field areVRStereoViewMatricesWithinSingleCullTolerance bool
---@field stereoTargetEye UnityEngine.StereoTargetEyeMask
---@field stereoActiveEye UnityEngine.Camera.MonoOrStereoscopicEye
---@field allCamerasCount int
---@field allCameras table
---@field sceneViewFilterMode UnityEngine.Camera.SceneViewFilterMode
---@field commandBufferCount int
---@field onPreCull UnityEngine.Camera.CameraCallback
---@field onPreRender UnityEngine.Camera.CameraCallback
---@field onPostRender UnityEngine.Camera.CameraCallback
local m = {}
function m:Reset() end
function m:ResetTransparencySortSettings() end
function m:ResetAspect() end
function m:ResetCullingMatrix() end
---@param shader UnityEngine.Shader
---@param replacementTag string
function m:SetReplacementShader(shader, replacementTag) end
function m:ResetReplacementShader() end
---@return float
function m:GetGateFittedFieldOfView() end
---@return UnityEngine.Vector2
function m:GetGateFittedLensShift() end
---@overload fun(colorBuffer:table, depthBuffer:UnityEngine.RenderBuffer):void
---@param colorBuffer UnityEngine.RenderBuffer
---@param depthBuffer UnityEngine.RenderBuffer
function m:SetTargetBuffers(colorBuffer, depthBuffer) end
function m:ResetWorldToCameraMatrix() end
function m:ResetProjectionMatrix() end
---@param clipPlane UnityEngine.Vector4
---@return UnityEngine.Matrix4x4
function m:CalculateObliqueMatrix(clipPlane) end
---@overload fun(position:UnityEngine.Vector3):UnityEngine.Vector3
---@param position UnityEngine.Vector3
---@param eye UnityEngine.Camera.MonoOrStereoscopicEye
---@return UnityEngine.Vector3
function m:WorldToScreenPoint(position, eye) end
---@overload fun(position:UnityEngine.Vector3):UnityEngine.Vector3
---@param position UnityEngine.Vector3
---@param eye UnityEngine.Camera.MonoOrStereoscopicEye
---@return UnityEngine.Vector3
function m:WorldToViewportPoint(position, eye) end
---@overload fun(position:UnityEngine.Vector3):UnityEngine.Vector3
---@param position UnityEngine.Vector3
---@param eye UnityEngine.Camera.MonoOrStereoscopicEye
---@return UnityEngine.Vector3
function m:ViewportToWorldPoint(position, eye) end
---@overload fun(position:UnityEngine.Vector3):UnityEngine.Vector3
---@param position UnityEngine.Vector3
---@param eye UnityEngine.Camera.MonoOrStereoscopicEye
---@return UnityEngine.Vector3
function m:ScreenToWorldPoint(position, eye) end
---@param position UnityEngine.Vector3
---@return UnityEngine.Vector3
function m:ScreenToViewportPoint(position) end
---@param position UnityEngine.Vector3
---@return UnityEngine.Vector3
function m:ViewportToScreenPoint(position) end
---@overload fun(pos:UnityEngine.Vector3):UnityEngine.Ray
---@param pos UnityEngine.Vector3
---@param eye UnityEngine.Camera.MonoOrStereoscopicEye
---@return UnityEngine.Ray
function m:ViewportPointToRay(pos, eye) end
---@overload fun(pos:UnityEngine.Vector3):UnityEngine.Ray
---@param pos UnityEngine.Vector3
---@param eye UnityEngine.Camera.MonoOrStereoscopicEye
---@return UnityEngine.Ray
function m:ScreenPointToRay(pos, eye) end
---@param viewport UnityEngine.Rect
---@param z float
---@param eye UnityEngine.Camera.MonoOrStereoscopicEye
---@param outCorners table
function m:CalculateFrustumCorners(viewport, z, eye, outCorners) end
---@param output UnityEngine.Matrix4x4
---@param focalLength float
---@param sensorSize UnityEngine.Vector2
---@param lensShift UnityEngine.Vector2
---@param nearClip float
---@param farClip float
---@param gateFitParameters UnityEngine.Camera.GateFitParameters
function m.CalculateProjectionMatrixFromPhysicalProperties(output, focalLength, sensorSize, lensShift, nearClip, farClip, gateFitParameters) end
---@param focalLength float
---@param sensorSize float
---@return float
function m.FocalLengthToFieldOfView(focalLength, sensorSize) end
---@param fieldOfView float
---@param sensorSize float
---@return float
function m.FieldOfViewToFocalLength(fieldOfView, sensorSize) end
---@param horizontalFieldOfView float
---@param aspectRatio float
---@return float
function m.HorizontalToVerticalFieldOfView(horizontalFieldOfView, aspectRatio) end
---@param verticalFieldOfView float
---@param aspectRatio float
---@return float
function m.VerticalToHorizontalFieldOfView(verticalFieldOfView, aspectRatio) end
---@param eye UnityEngine.Camera.StereoscopicEye
---@return UnityEngine.Matrix4x4
function m:GetStereoNonJitteredProjectionMatrix(eye) end
---@param eye UnityEngine.Camera.StereoscopicEye
---@return UnityEngine.Matrix4x4
function m:GetStereoViewMatrix(eye) end
---@param eye UnityEngine.Camera.StereoscopicEye
function m:CopyStereoDeviceProjectionMatrixToNonJittered(eye) end
---@param eye UnityEngine.Camera.StereoscopicEye
---@return UnityEngine.Matrix4x4
function m:GetStereoProjectionMatrix(eye) end
---@param eye UnityEngine.Camera.StereoscopicEye
---@param matrix UnityEngine.Matrix4x4
function m:SetStereoProjectionMatrix(eye, matrix) end
function m:ResetStereoProjectionMatrices() end
---@param eye UnityEngine.Camera.StereoscopicEye
---@param matrix UnityEngine.Matrix4x4
function m:SetStereoViewMatrix(eye, matrix) end
function m:ResetStereoViewMatrices() end
---@param cameras table
---@return int
function m.GetAllCameras(cameras) end
---@overload fun(cubemap:UnityEngine.Cubemap):bool
---@overload fun(cubemap:UnityEngine.RenderTexture, faceMask:int):bool
---@overload fun(cubemap:UnityEngine.RenderTexture):bool
---@overload fun(cubemap:UnityEngine.RenderTexture, faceMask:int, stereoEye:UnityEngine.Camera.MonoOrStereoscopicEye):bool
---@param cubemap UnityEngine.Cubemap
---@param faceMask int
---@return bool
function m:RenderToCubemap(cubemap, faceMask) end
function m:Render() end
---@param shader UnityEngine.Shader
---@param replacementTag string
function m:RenderWithShader(shader, replacementTag) end
function m:RenderDontRestore() end
---@param renderRequests table
function m:SubmitRenderRequests(renderRequests) end
---@param cur UnityEngine.Camera
function m.SetupCurrent(cur) end
---@param other UnityEngine.Camera
function m:CopyFrom(other) end
---@param evt UnityEngine.Rendering.CameraEvent
function m:RemoveCommandBuffers(evt) end
function m:RemoveAllCommandBuffers() end
---@param evt UnityEngine.Rendering.CameraEvent
---@param buffer UnityEngine.Rendering.CommandBuffer
function m:AddCommandBuffer(evt, buffer) end
---@param evt UnityEngine.Rendering.CameraEvent
---@param buffer UnityEngine.Rendering.CommandBuffer
---@param queueType UnityEngine.Rendering.ComputeQueueType
function m:AddCommandBufferAsync(evt, buffer, queueType) end
---@param evt UnityEngine.Rendering.CameraEvent
---@param buffer UnityEngine.Rendering.CommandBuffer
function m:RemoveCommandBuffer(evt, buffer) end
---@param evt UnityEngine.Rendering.CameraEvent
---@return table
function m:GetCommandBuffers(evt) end
---@overload fun(stereoAware:bool, cullingParameters:UnityEngine.Rendering.ScriptableCullingParameters):bool
---@param cullingParameters UnityEngine.Rendering.ScriptableCullingParameters
---@return bool
function m:TryGetCullingParameters(cullingParameters) end
UnityEngine = {}
UnityEngine.Camera = m
return m