using UnityEngine.Rendering;
#if HAS_URP
using ImGuiNET;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
#endif

namespace UImGui.Renderer
{
#if HAS_URP
	public class RenderImGui : ScriptableRendererFeature
	{
		private class PassData
		{
			internal UImGui uimgui;
			internal ImDrawDataPtr drawData;
		}

		private class ImGuiRenderPass : ScriptableRenderPass
		{
			private RenderImGui _feature;

			public void Setup(RenderImGui feature)
			{
				_feature = feature;
			}

			public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
			{
				if (_feature == null || _feature._uimgui == null)
				{
					return;
				}

				UImGui uimgui = _feature._uimgui;

				UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
				UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

				if (cameraData.camera.pixelWidth == 0 || cameraData.camera.pixelHeight == 0)
				{
					return;
				}

				ImDrawDataPtr drawData = ImGui.GetDrawData();
				if (!drawData.Valid || drawData.TotalVtxCount == 0)
					return;

				using (var builder = renderGraph.AddRasterRenderPass<PassData>("ImGui Render", out var passData))
				{
					// Set color and depth attachments
					builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
					builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Read);
					builder.AllowPassCulling(false);

					passData.uimgui = uimgui;
					passData.drawData = drawData;

					builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
					{
						var renderer = (RendererMesh)data.uimgui.GetRenderer();
						renderer.RenderDrawListsRG(context.cmd, data.drawData);
					});
				}
			}
		}

		[HideInInspector]
		public Camera Camera;
		public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

		private ImGuiRenderPass _renderPass;
		internal UImGui _uimgui;

		public void SetUImGui(UImGui uimgui, Camera camera)
		{
			_uimgui = uimgui;
			Camera = camera;
		}

		public override void Create()
		{
			_renderPass = new ImGuiRenderPass
			{
				renderPassEvent = RenderPassEvent
			};
			_renderPass.Setup(this);
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (_renderPass == null || Camera != renderingData.cameraData.camera)
				return;

			_renderPass.renderPassEvent = RenderPassEvent;
			renderer.EnqueuePass(_renderPass);
		}
	}
#else
	public class RenderImGui : UnityEngine.ScriptableObject
	{
	}
#endif
}
