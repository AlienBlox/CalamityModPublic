using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Sounds.Item
{
    public class TankCannon : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
        {
            soundInstance = sound.CreateInstance();
            soundInstance.Volume = volume * 1.1f;
            soundInstance.Pan = pan;
            soundInstance.Pitch = (float)Main.rand.Next(-25, 26) * 0.01f;
            Main.PlaySoundInstance(soundInstance);
            return soundInstance;
        }
    }
}
