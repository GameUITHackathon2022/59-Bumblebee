namespace Dialogue {
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ShakeAnimation : TextAnimation
    {
        [SerializeField]
        [Tooltip("The library of ShakePresets that can be used by this component.")]
        private ShakeLibrary _shakeLibrary;

        [SerializeField]
        [Tooltip("The name (key) of the ShakePreset this animation should use.")]
        private string _shakePresetKey;

        private ShakePreset _shakePreset;

        /// <summary>
        /// Load a particular ShakePreset animation into this Component
        /// </summary>
        /// <param name="library">The library of ShakePresets that can be used by this component</param>
        /// <param name="presetKey">The name (key) of the ShakePreset this animation should use</param>
        public void LoadPreset(ShakeLibrary library, string presetKey)
        {
            this._shakeLibrary = library;
            this._shakePresetKey = presetKey;
            this._shakePreset = library[presetKey];
        }

        protected override void OnEnable()
        {
            if (this._shakeLibrary != null && !string.IsNullOrEmpty(this._shakePresetKey))
            {
                LoadPreset(this._shakeLibrary, this._shakePresetKey);
            }

            base.OnEnable();
        }

        protected override void Animate(int characterIndex, out Vector2 translation, out float rotation, out float scale)
        {
            translation = Vector2.zero;
            rotation = 0f;
            scale = 1f;

            // Do nothing if a ShakePreset has not been configured yet
            if (this._shakePreset == null)
            {
                return;
            }

            if (characterIndex >= this.firstCharToAnimate && characterIndex <= this.lastCharToAnimate)
            {
                float randomX = Random.Range(-this._shakePreset.xPosStrength, this._shakePreset.xPosStrength);
                float randomY = Random.Range(-this._shakePreset.yPosStrength, this._shakePreset.yPosStrength);
                translation = new Vector2(randomX, randomY);

                rotation = Random.Range(-this._shakePreset.rotationStrength, this._shakePreset.rotationStrength);

                scale = 1f + Random.Range(-this._shakePreset.scaleStrength, this._shakePreset.scaleStrength);
            }
        }
    }
}