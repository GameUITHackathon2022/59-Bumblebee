namespace Dialogue
{
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CurveAnimation : TextAnimation
    {
        [SerializeField]
        [Tooltip("The library of CurvePresets that can be used by this component.")]
        private CurveLibrary _curveLibrary;

        [SerializeField]
        [Tooltip("The name (key) of the CurvePreset this animation should use.")]
        private string _curvePresetKey;

        private CurvePreset _curvePreset;

        private float timeAnimationStarted;

        /// <summary>
        /// Load a particular CurvePreset animation into this Component
        /// </summary>
        /// <param name="library">The library of CurvePresets that can be used by this component</param>
        /// <param name="presetKey">The name (key) of the CurvePreset this animation should use</param>
        public void LoadPreset(CurveLibrary library, string presetKey)
        {
            this._curveLibrary = library;
            this._curvePresetKey = presetKey;
            this._curvePreset = library[presetKey];
        }

        protected override void OnEnable()
        {
            if (this._curveLibrary != null && !string.IsNullOrEmpty(this._curvePresetKey))
            {
                LoadPreset(this._curveLibrary, this._curvePresetKey);
            }

            this.timeAnimationStarted = this.timeForTimeScale;
            base.OnEnable();
        }

        protected override void Animate(int characterIndex, out Vector2 translation, out float rotation, out float scale)
        {
            translation = Vector2.zero;
            rotation = 0f;
            scale = 1f;

            // Do nothing if a CurvePreset has not been configured yet
            if (this._curvePreset == null)
            {
                return;
            }

            if (characterIndex >= this.firstCharToAnimate && characterIndex <= this.lastCharToAnimate)
            {
                // Calculate a t based on time since the animation started, 
                // but offset per character (to produce wave effects)
                float t = this.timeForTimeScale - this.timeAnimationStarted + (characterIndex * this._curvePreset.timeOffsetPerChar);

                float xPos = this._curvePreset.xPosCurve.Evaluate(t) * this._curvePreset.xPosMultiplier;
                float yPos = this._curvePreset.yPosCurve.Evaluate(t) * this._curvePreset.yPosMultiplier;

                translation = new Vector2(xPos, yPos);

                rotation = this._curvePreset.rotationCurve.Evaluate(t) * this._curvePreset.rotationMultiplier;
                scale += this._curvePreset.scaleCurve.Evaluate(t) * this._curvePreset.scaleMultiplier;
            }
        }
    }
}