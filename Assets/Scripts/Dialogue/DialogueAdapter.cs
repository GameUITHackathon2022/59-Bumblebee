namespace Dialogue
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using MonsterLove.StateMachine;

    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class DialogueAdapter : MonoBehaviour
    {
        private const float PRINT_DELAY_DEFAULT = 0.02f;
        private const float PUNCTUATION_DELAY_FACTOR = 8f;
        private static readonly List<char> PunctutationCharacters = new List<char>
        {
            '.',
            ',',
            '!',
            '?'
        };

#pragma warning disable 0649
        [SerializeField]
        [Tooltip("The library of ShakePreset animations that can be used by this component.")]
        private ShakeLibrary _shakeLibrary;

        [SerializeField]
        [Tooltip("The library of CurvePreset animations that can be used by this component.")]
        private CurveLibrary _curveLibrary;

        [SerializeField]
        [Tooltip("If set, the typer will type text even if the game is paused (Time.timeScale = 0)")]
        private bool _useUnscaledTime;
#pragma warning restore 0649

        [SerializeField]
        [Tooltip("Event that's called when the text has finished printing.")]
        private TextPrintedEvent _printCompleted = new TextPrintedEvent();

        [SerializeField]
        [Tooltip("Event called when a character is printed. Inteded for audio callbacks.")]
        private CharacterPrintedEvent _characterPrinted = new CharacterPrintedEvent();

        [SerializeField]
        [Tooltip("The dialogue that this UGUI is using to display.")]
        private DialogueScriptableObject _dialogue;

        [SerializeField]
        [Tooltip("Event called when the dialogue begins display.")]
        private DialogueEvent _dialogueBegun = new DialogueEvent();

        [SerializeField]
        [Tooltip("Event called when the dialogue ends.")]
        private DialogueEvent _dialogueEnded = new DialogueEvent();

        private TextMeshProUGUI _textComponent;
        private float _defaultPrintDelay;
        private List<TypableCharacter> _charactersToType;
        private List<TextAnimation> _animations;
        private Coroutine _typeTextCoroutine;
        private int _currentDialogueIndex = 0;

        public TextPrintedEvent PrintCompleted
        {
            get
            {
                return this._printCompleted;
            }
        }

        public CharacterPrintedEvent CharacterPrinted
        {
            get
            {
                return this._characterPrinted;
            }
        }

        public DialogueEvent DialogueBegin
        {
            get
            {
                return this._dialogueBegun;
            }
        }

        public DialogueEvent DialogueEnd
        {
            get
            {
                return this._dialogueEnded;
            }
        }

        public bool IsTyping
        {
            get
            {
                return this._typeTextCoroutine != null;
            }
        }

        private TextMeshProUGUI TextComponent
        {
            get
            {
                if (this._textComponent == null)
                {
                    this._textComponent = this.GetComponent<TextMeshProUGUI>();
                }

                return this._textComponent;
            }
        }

        public enum DialogueStates {
            INIT,
            IDLE,
            TYPING,
            END
        }

        private StateMachine<DialogueStates> _dialogueStateMachine;

        private void Awake()
        {
            _dialogueStateMachine = new StateMachine<DialogueStates>(this);
            _dialogueStateMachine.ChangeState(DialogueStates.INIT);
        }

        public void StartDialogue() {
            _currentDialogueIndex = 0;
            DialogueBegin.Invoke(_dialogue.key);
            ForwardDialogue();
        }

        public void ForwardDialogue()
        {
            switch(_dialogueStateMachine.State) {
                case DialogueStates.INIT: 
                    _dialogueStateMachine.ChangeState(DialogueStates.TYPING);
                    break;
                case DialogueStates.IDLE:
                    _currentDialogueIndex++;
                    if (_dialogue.GetTextObject(_currentDialogueIndex) == null) {
                        _dialogueStateMachine.ChangeState(DialogueStates.END);
                    } else {
                        _dialogueStateMachine.ChangeState(DialogueStates.TYPING);
                    }
                    break;
                case DialogueStates.TYPING:
                    _dialogueStateMachine.ChangeState(DialogueStates.IDLE);
                    break;
                case DialogueStates.END:
                    Debug.LogError("This dialogue has already ended!");
                    break;
                default:
                    break;
            }
        }

        public void EndDialogue() {
            _dialogueStateMachine.ChangeState(DialogueStates.END);
            DialogueEnd.Invoke(_dialogue.key);
        }

        private void INIT_Enter() {
            _currentDialogueIndex = 0;
        }

        private void TYPING_Enter()
        {
            Debug.Log("This has been ran");
            string text = _dialogue.GetTextObject(_currentDialogueIndex).content;
            int printDelay = -1;
            this.CleanupCoroutine();

            foreach (var anim in GetComponents<TextAnimation>())
            {
                Destroy(anim);
            }

            this._defaultPrintDelay = printDelay > 0 ? printDelay : PRINT_DELAY_DEFAULT;
            this.ProcessTags(text);

            // Fix Issue-38 by clearing any old textInfo like sprites, so that SubMesh objects don't reshow their contents.
            var textInfo = this.TextComponent.textInfo;
            textInfo.ClearMeshInfo(false);

            this._typeTextCoroutine = this.StartCoroutine(this.TypeTextCharByChar(text));
        }

        private void TYPING_Exit()
        {
            this.CleanupCoroutine();

            this.TextComponent.maxVisibleCharacters = int.MaxValue;
            this.UpdateMeshAndAnims();

            this.OnTypewritingComplete(_currentDialogueIndex);
        }

        private bool IsSkippable()
        {
            return this.IsTyping;
        }

        private void CleanupCoroutine()
        {
            if (this._typeTextCoroutine != null)
            {
                this.StopCoroutine(this._typeTextCoroutine);
                this._typeTextCoroutine = null;
            }
        }

        private IEnumerator TypeTextCharByChar(string text)
        {
            Debug.Log("Typed text here");
            this.TextComponent.SetText(TextTagParser.RemoveCustomTags(text));
            for (int numPrintedCharacters = 0; numPrintedCharacters < this._charactersToType.Count; ++numPrintedCharacters)
            {
                this.TextComponent.maxVisibleCharacters = numPrintedCharacters + 1;
                this.UpdateMeshAndAnims();

                var printedChar = this._charactersToType[numPrintedCharacters];
                this.OnCharacterPrinted(printedChar.ToString());

                if (this._useUnscaledTime)
                {
                    yield return new WaitForSecondsRealtime(printedChar.delay);
                }
                else
                {
                    yield return new WaitForSeconds(printedChar.delay);
                }
            }

            this._typeTextCoroutine = null;
            this.OnTypewritingComplete(_currentDialogueIndex);
        }

        private void UpdateMeshAndAnims()
        {
            this.TextComponent.ForceMeshUpdate();
            for (int i = 0; i < this._animations.Count; i++)
            {
                this._animations[i].AnimateAllChars();
            }
        }

        private void ProcessTags(string text)
        {
            this._charactersToType = new List<TypableCharacter>();
            this._animations = new List<TextAnimation>();
            var textAsSymbolList = TextTagParser.CreateSymbolListFromText(text);

            int printedCharCount = 0;
            int customTagOpenIndex = 0;
            string customTagParam = "";
            float nextDelay = this._defaultPrintDelay;
            foreach (var symbol in textAsSymbolList)
            {
                if (symbol.IsTag && !symbol.IsReplacedWithSprite)
                {
                    if (symbol.Tag.TagType == TextTagParser.CustomTags.Delay)
                    {
                        if (symbol.Tag.IsClosingTag)
                        {
                            nextDelay = this._defaultPrintDelay;
                        }
                        else
                        {
                            nextDelay = symbol.GetFloatParameter(this._defaultPrintDelay);
                        }
                    }
                    else if (symbol.Tag.TagType == TextTagParser.CustomTags.Anim ||
                             symbol.Tag.TagType == TextTagParser.CustomTags.Animation)
                    {
                        if (symbol.Tag.IsClosingTag)
                        {
                            TextAnimation anim = null;
                            if (this.IsAnimationShake(customTagParam))
                            {
                                anim = gameObject.AddComponent<ShakeAnimation>();
                                ((ShakeAnimation)anim).LoadPreset(this._shakeLibrary, customTagParam);
                            }
                            else if (this.IsAnimationCurve(customTagParam))
                            {
                                anim = gameObject.AddComponent<CurveAnimation>();
                                ((CurveAnimation)anim).LoadPreset(this._curveLibrary, customTagParam);
                            }
                            else
                            {
                                // Error
                            }

                            anim.useUnscaledTime = this._useUnscaledTime;
                            anim.SetCharsToAnimate(customTagOpenIndex, printedCharCount - 1);
                            anim.enabled = true;
                            this._animations.Add(anim);
                        }
                        else
                        {
                            customTagOpenIndex = printedCharCount;
                            customTagParam = symbol.Tag.Parameter;
                        }
                    }
                    else
                    {
                        // Unrecognized
                    }

                }
                else
                {
                    printedCharCount++;

                    TypableCharacter characterToType = new TypableCharacter();
                    if (symbol.IsTag && symbol.IsReplacedWithSprite)
                    {
                        characterToType.isSprite = true;
                    }
                    else
                    {
                        characterToType.character = symbol.Character;
                    }

                    characterToType.delay = nextDelay;
                    if (PunctutationCharacters.Contains(symbol.Character))
                    {
                        characterToType.delay *= PUNCTUATION_DELAY_FACTOR;
                    }

                    this._charactersToType.Add(characterToType);
                }
            }
        }

        private bool IsAnimationShake(string animName)
        {
            return this._shakeLibrary.ContainsKey(animName);
        }

        private bool IsAnimationCurve(string animName)
        {
            return this._curveLibrary.ContainsKey(animName);
        }

        private void OnCharacterPrinted(string printedCharacter)
        {
            if (this.CharacterPrinted != null)
            {
                this.CharacterPrinted.Invoke(printedCharacter);
            }
        }

        private void OnTypewritingComplete(int index)
        {
            if (this.PrintCompleted != null)
            {
                this.PrintCompleted.Invoke(index);
            }
        }

        [System.Serializable]
        public class CharacterPrintedEvent : UnityEvent<string> {}

        [System.Serializable]
        public class TextPrintedEvent : UnityEvent<int> {}

        [System.Serializable]
        public class DialogueEvent : UnityEvent<string> {}

        private class TypableCharacter
        {
            public char character { get; set; }

            public float delay { get; set; }

            public bool isSprite { get; set; }

            public override string ToString()
            {
                return this.isSprite ? "Sprite" : character.ToString();
            }
        }
    }
}