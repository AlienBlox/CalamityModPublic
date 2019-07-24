using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Sounds.NPCKilled
{
	public class Sunskater : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			soundInstance = sound.CreateInstance();
			soundInstance.Volume = volume * 2f; //1f
			soundInstance.Pan = pan;
			Main.PlaySoundInstance(soundInstance);
			return soundInstance;
		}
	}
}