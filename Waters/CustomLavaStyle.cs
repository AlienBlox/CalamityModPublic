using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
	public abstract class CustomLavaStyle
	{
		internal Texture2D LavaTexture;
		internal Texture2D BlockTexture;

		internal void Load()
        {
			// Don't load textures serverside.
			if (Main.netMode == NetmodeID.Server)
				return;

			LavaTexture = ModContent.GetTexture(LavaTexturePath);
			BlockTexture = ModContent.GetTexture(BlockTexturePath);
		}

		internal void Unload()
        {
			LavaTexture = null;
			BlockTexture = null;
		}

		public abstract string LavaTexturePath { get; }

		public abstract string BlockTexturePath { get; }

		public virtual bool ChooseLavaStyle() => false;

		public abstract int ChooseWaterfallStyle();

		public abstract int GetSplashDust();

		public abstract int GetDropletGore();

		public virtual void SelectLightColor(ref Color initialLightColor)
		{
		}
	}
}
