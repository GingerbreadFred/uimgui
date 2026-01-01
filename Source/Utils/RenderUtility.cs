using UnityEngine.Rendering;
#if HAS_URP
using UnityEngine.Rendering.Universal;
#elif HAS_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif

namespace UImGui
{
	internal static class RenderUtility
	{
		public static bool IsUsingURP()
		{
			RenderPipelineAsset currentRP = GraphicsSettings.currentRenderPipeline;
#if HAS_URP
			return currentRP is UniversalRenderPipelineAsset;
#else
			return false;
#endif
		}

		public static bool IsUsingHDRP()
		{
			RenderPipelineAsset currentRP = GraphicsSettings.currentRenderPipeline;

#if HAS_HDRP
			return currentRP is HDRenderPipelineAsset;
#else
			return false;
#endif
		}
	}
}
