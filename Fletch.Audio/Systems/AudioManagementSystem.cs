using Fletch.Audio.Components;
using Fletch.Engine.Components;
using Fletch.Engine.Model;
using Fletch.Engine.Scenes;
using Fletch.Engine.Systems;

namespace Fletch.Audio.Systems
{
    internal sealed class AudioManagementSystem : SceneSubsystem
    {
        public override void AttachToScene(Scene scene)
        {
            base.AttachToScene(scene);

            scene.AddSystemComponentRegistration(typeof(AudioSource), (b, c) => OnAudioSourceChange(b, c));
            scene.AddSystemComponentRegistration(typeof(AudioListener), (b, c) => OnAudioListenerChange(b, c));
        }

        private void OnAudioSourceChange(GameObjectComponent component, ComponentChangeType changeType)
        {
            AudioSource source = (AudioSource)component;

        }

        private void OnAudioListenerChange(GameObjectComponent component, ComponentChangeType changeType)
        {
            AudioListener listener = (AudioListener)component;

        }
    }
}
