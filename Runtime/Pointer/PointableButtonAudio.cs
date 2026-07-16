using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class PointableButtonAudio
    {
        [SerializeField] protected AudioSource _audioSource = null;
        [SerializeField] protected AudioClip   _pointed     = null;
        [SerializeField] protected AudioClip   _unpointed   = null;
        [SerializeField] protected AudioClip   _pressed     = null;
        [SerializeField] protected AudioClip   _unpressed   = null;
        [SerializeField] protected AudioClip   _cancelled   = null;
        [SerializeField] protected AudioClip   _repressed   = null;
        [SerializeField] protected AudioClip   _enabled     = null;
        [SerializeField] protected AudioClip   _disabled    = null;

        public void Pointed()
        {
            if (!_audioSource)
                return;

            if (!_pointed)
                return;
            
            _audioSource.PlayOneShot(_pointed);
        }

        public void Unpointed()
        {
            if (!_audioSource)
                return;

            if (!_unpointed)
                return;
            
            _audioSource.PlayOneShot(_unpointed);
        }

        public void Pressed(bool repress)
        {
            if (!_audioSource)
                return;

            if (repress)
            {
                if (!_repressed)
                    return;

                _audioSource.PlayOneShot(_repressed);
                return;
            }

            if (!_pressed)
                return;

            _audioSource.PlayOneShot(_pressed);
        }

        public void Unpressed(bool cancel)
        {
            if (!_audioSource)
                return;

            if (cancel)
            {
                if (!_cancelled)
                    return;

                _audioSource.PlayOneShot(_cancelled);
                return;
            }

            if (!_unpressed)
                return;

            _audioSource.PlayOneShot(_unpressed);
        }

        public void Enabled()
        {
            if (!_audioSource)
                return;

            if (!_enabled)
                return;
            
            _audioSource.PlayOneShot(_enabled);
        }

        public void Disabled()
        {
            if (!_audioSource)
                return;

            if (!_disabled)
                return;
            
            _audioSource.PlayOneShot(_disabled);
        }
    }
}