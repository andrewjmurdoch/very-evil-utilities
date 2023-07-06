using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VED.Utilities
{
    public class DebugManager : SingletonMonoBehaviour<DebugManager>
    {
        [SerializeField] private bool _enabled = true;
        [SerializeField] private bool _open = false;

        private const int MARGIN = 40;
        private const float HEIGHT = 0.05f;
        private const float WIDTH  = 0.45f;
        private Vector2 _scrollViewScrollPosition = Vector2.zero;

        private GUILayoutOption[] _options = { };

        private const float FONT_SCREEN_RATIO = 40f;

        private GUIStyle _styleBackground = GUIStyle.none;
        private GUIStyle _styleMargin = GUIStyle.none;
        private GUIStyle _styleLabel = GUIStyle.none;
        private GUIStyle _styleButton = GUIStyle.none;
        private GUIStyle _styleText = GUIStyle.none;
        private GUIStyle _styleTextHalf = GUIStyle.none;
        private GUIStyle _styleToggleOn = GUIStyle.none;
        private GUIStyle _styleToggleOff = GUIStyle.none;
        private GUIStyle _styleSlider = GUIStyle.none;
        private GUIStyle _styleSliderThumb = GUIStyle.none;

        public Action OnDebugMenuRender = null;
        public Action OnDebugMenuOpen   = null;
        public Action OnDebugMenuClose  = null;

        private void OnGUI()
        {
#if UNITY_EDITOR
            if (!_enabled) return;

            int fontSize = (int)(Screen.width / FONT_SCREEN_RATIO);

            _styleBackground = new GUIStyle(GUI.skin.textField);

            _styleMargin = new GUIStyle(GUI.skin.label);
            _styleMargin.fixedHeight = Screen.height * HEIGHT * 0.2f;
            _styleMargin.fixedWidth = Screen.width * WIDTH;

            _styleLabel = new GUIStyle(GUI.skin.label);
            _styleLabel.fontSize = fontSize;
            _styleLabel.fixedHeight = Screen.height * HEIGHT;
            _styleLabel.fixedWidth  = Screen.width  * WIDTH;

            _styleButton = new GUIStyle(GUI.skin.button);
            _styleButton.fontSize = fontSize;
            _styleButton.fixedHeight = Screen.height * HEIGHT;
            _styleButton.fixedWidth   = Screen.width  * WIDTH;

            _styleText = new GUIStyle(GUI.skin.textField);
            _styleText.fontSize = fontSize;
            _styleText.fixedHeight = Screen.height * HEIGHT;
            _styleText.fixedWidth = Screen.width * WIDTH;

            _styleTextHalf = new GUIStyle(_styleText);
            _styleTextHalf.fixedWidth = Screen.width * (WIDTH / 2.3f);

            _styleToggleOn = new GUIStyle(GUI.skin.button);
            _styleToggleOn.fontSize = fontSize;
            _styleToggleOn.normal.textColor = Color.green;
            _styleToggleOn.fixedHeight = Screen.height * HEIGHT;
            _styleToggleOn.fixedWidth = Screen.width * WIDTH;

            _styleToggleOff = new GUIStyle(GUI.skin.button);
            _styleToggleOff.fontSize = fontSize;
            _styleToggleOff.normal.textColor = Color.red;
            _styleToggleOff.fixedHeight = Screen.height * HEIGHT;
            _styleToggleOff.fixedWidth = Screen.width * WIDTH;

            _styleSlider = new GUIStyle(GUI.skin.horizontalSlider);
            _styleSlider.fontSize = fontSize;
            _styleSlider.fixedWidth = Screen.width * WIDTH;
            _styleSlider.fixedHeight = Screen.height * 0.035f;

            _styleSliderThumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
            _styleSliderThumb.fontSize = fontSize;
            _styleSliderThumb.fixedWidth = _styleSlider.fixedWidth / 10f;
            _styleSliderThumb.fixedHeight = _styleSlider.fixedHeight;

            if (!_open)
            {
                if (GUILayout.Button("Open Debug Menu", _styleButton))
                {
                    _open = true;
                    OnDebugMenuOpen?.Invoke();
                    Time.timeScale = 0f;
                }
                return;
            }

            GUILayout.Box("", _styleBackground, new GUILayoutOption[] { GUILayout.Width(Screen.width), GUILayout.Height(Screen.height) });
            GUILayout.BeginArea(new Rect(MARGIN, MARGIN, Screen.width - (MARGIN * 2), Screen.height - (MARGIN * 2)));
            GUILayout.Box("Debug Menu", _styleLabel);
            if (GUILayout.Button("Close Debug Menu", _styleButton))
            {
                _open = false;
                OnDebugMenuClose?.Invoke();
                Time.timeScale = 1f;
            }

            GUI.skin.verticalScrollbar.fixedWidth = Screen.width * 0.025f;
            GUI.skin.verticalScrollbarThumb.fixedWidth = Screen.width * 0.025f;
            _scrollViewScrollPosition = GUILayout.BeginScrollView(_scrollViewScrollPosition, false, true, new GUIStyle(GUI.skin.horizontalScrollbar), new GUIStyle(GUI.skin.verticalScrollbar), new GUILayoutOption[] { GUILayout.Width(Screen.width - (MARGIN * 2)), GUILayout.MaxHeight(Screen.height - (GUIStyle.none.lineHeight * 2f) - (MARGIN * 2)) });

            OnDebugMenuRender?.Invoke();

            GUILayout.EndScrollView();
            GUILayout.EndArea();
#endif
        }

        private Texture2D CreateTexture(int width, int height, Color colour)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = colour;
            }
            Texture2D texture2D = new Texture2D(width, height);
            texture2D.SetPixels(pixels);
            texture2D.Apply();
            return texture2D;
        }

        private static void MarginField()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Empty, Instance._styleMargin);
            GUILayout.EndHorizontal();
        }

        public static void LabelField(string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(text, Instance._styleLabel);
            GUILayout.EndHorizontal();
            MarginField();
        }

        public static void SpacerField()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Empty, Instance._styleLabel);
            GUILayout.EndHorizontal();
        }

        public static string TextField(string name, string value, Action<string> onChange = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, Instance._styleLabel);
            GUILayout.BeginVertical();

            string newValue = GUILayout.TextField(value, Instance._styleText, Instance._options);
            if (newValue != value)
            {
                onChange?.Invoke(value);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            MarginField();

            return newValue;
        }

        public static int IntField(string name, int value, Action<int> onChange = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, Instance._styleLabel);
            GUILayout.BeginVertical();

            string newValue = GUILayout.TextField(value.ToString(), Instance._styleText, Instance._options);

            if (newValue.Length == 0) newValue = "0";

            if (int.TryParse(newValue, out int newIntValue))
            {
                if (newIntValue != value)
                {
                    value = newIntValue;
                    onChange?.Invoke(newIntValue);
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            MarginField();

            return value;
        }

        public static (int one, int two) IntTupleField(string name, int one, int two, Action<int> onOneChange = null, Action<int> onTwoChange = null)
        {
            (int one, int two) value = (one, two);

            GUILayout.BeginHorizontal();
            GUILayout.Label(name, Instance._styleLabel);
            GUILayout.BeginVertical();
            string newValue = GUILayout.TextField(one.ToString(), Instance._styleTextHalf, Instance._options);
            if (newValue.Length == 0) newValue = "0";

            if (int.TryParse(newValue, out int newIntValue))
            {
                if (newIntValue != one)
                {
                    value.one = newIntValue;
                    onOneChange?.Invoke(newIntValue);
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            newValue = GUILayout.TextField(two.ToString(), Instance._styleTextHalf, Instance._options);
            if (newValue.Length == 0) newValue = "0";

            if (int.TryParse(newValue, out newIntValue))
            {
                if (newIntValue != two)
                {
                    value.two = newIntValue;
                    onTwoChange?.Invoke(newIntValue);
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            MarginField();

            return value;
        }

        public static int IntButtonField(string name, int value, Action<int> onPress = null)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(name, Instance._styleButton, Instance._options))
            {
                onPress?.Invoke(value);
            }
            GUILayout.BeginVertical();

            string newValue = GUILayout.TextField(value.ToString(), Instance._styleText, Instance._options);

            if (newValue.Length == 0) newValue = "0";

            if (int.TryParse(newValue, out int newIntValue))
            {
                if (newIntValue != value)
                {
                    value = newIntValue;
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            MarginField();

            return value;
        }

        public static float FloatField(string name, float value, Action<float> onChange = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, Instance._styleLabel);
            GUILayout.BeginVertical();

            string newValue = GUILayout.TextField(String.Format("{0:00.0000}", value), Instance._styleText, Instance._options);

            if (newValue.Length == 0) newValue = "0";

            if (float.TryParse(newValue, out float newFloatValue))
            {
                if (newFloatValue != value)
                {
                    value = newFloatValue;
                    onChange?.Invoke(newFloatValue);
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            MarginField();

            return value;
        }

        public static (float one, float two) FloatTupleField(string name, float one, float two, Action<float> onOneChange = null, Action<float> onTwoChange = null)
        {
            (float one, float two) value = (one, two);

            GUILayout.BeginHorizontal();
            GUILayout.Label(name, Instance._styleLabel);
            GUILayout.BeginVertical();
            string newValue = GUILayout.TextField(one.ToString(), Instance._styleTextHalf, Instance._options);
            if (newValue.Length == 0) newValue = "0";

            if (float.TryParse(newValue, out float newFloatValue))
            {
                if (newFloatValue != one)
                {
                    value.one = newFloatValue;
                    onOneChange?.Invoke(newFloatValue);
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            newValue = GUILayout.TextField(two.ToString(), Instance._styleTextHalf, Instance._options);
            if (newValue.Length == 0) newValue = "0";

            if (float.TryParse(newValue, out newFloatValue))
            {
                if (newFloatValue != two)
                {
                    value.two = newFloatValue;
                    onTwoChange?.Invoke(newFloatValue);
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            MarginField();

            return value;
        }

        public static float FloatButtonField(string name, float value, Action<float> onPress = null)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(name, Instance._styleButton, Instance._options))
            {
                onPress?.Invoke(value);
            }
            GUILayout.BeginVertical();

            string newValue = GUILayout.TextField(String.Format("{0:00.0000}", value), Instance._styleText, Instance._options);

            if (newValue.Length == 0) newValue = "0";

            if (float.TryParse(newValue, out float newFloatValue))
            {
                if (newFloatValue != value)
                {
                    value = newFloatValue;
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            MarginField();

            return value;
        }

        public static bool BoolField(string name, bool value, Action<bool> onChange = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, Instance._styleLabel);
            GUILayout.BeginVertical();

            string text = value ? "on" : "off";
            GUIStyle style = value ? Instance._styleToggleOn : Instance._styleToggleOff;
            if (GUILayout.Button(text, style, Instance._options))
            {
                value = !value;
                onChange?.Invoke(value);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            return value;
        }

        public static void ButtonField(string name, Action onPress = null)
        {
            if (GUILayout.Button(name, Instance._styleButton, Instance._options))
            {
                onPress?.Invoke();
            }
            MarginField();
        }

        public static Color ColourField(string name, Color value, Action<Color> onChange = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, Instance._styleLabel);
            GUILayout.BeginVertical();

            GUIStyle styleColour = new GUIStyle(GUI.skin.box);
            styleColour.normal.background = Instance.CreateTexture(1, 1, value);
            GUILayout.Box(string.Empty, styleColour, new GUILayoutOption[] { GUILayout.Width(Screen.width * WIDTH), GUILayout.Height(Screen.height * 0.075f) });

            float r = GUILayout.HorizontalSlider(value.r, 0f, 1f, Instance._styleSlider, Instance._styleSliderThumb);
            float g = GUILayout.HorizontalSlider(value.g, 0f, 1f, Instance._styleSlider, Instance._styleSliderThumb);
            float b = GUILayout.HorizontalSlider(value.b, 0f, 1f, Instance._styleSlider, Instance._styleSliderThumb);
            float a = GUILayout.HorizontalSlider(value.a, 0f, 1f, Instance._styleSlider, Instance._styleSliderThumb);

            if (r != value.r || g != value.g || b != value.b || a != value.a)
            {
                value = new Color(r, g, b, a);
                onChange?.Invoke(value);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            MarginField();

            return value;
        }


        public static T EnumField<T>(string name, T value) where T : Enum
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, Instance._styleLabel);
            GUILayout.BeginVertical();

            GUILayout.TextField(value.ToString(), Instance._styleText, Instance._options);

            List<T> values = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            string[] names = Enum.GetNames(typeof(T));

            for (int i = 0; i < names.Length; i++)
            {
                if (GUILayout.Button(names[i], Instance._styleButton, Instance._options))
                {
                    value = values[i];
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            MarginField();

            return value;
        }

        public static T EnumButtonField<T>(string name, T value, Action<T> onPress = null) where T : Enum
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(name, Instance._styleButton, Instance._options))
            {
                onPress?.Invoke(value);
            }
            GUILayout.BeginVertical();

            GUILayout.TextField(value.ToString(), Instance._styleText, Instance._options);

            List<T> values = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            string[] names = Enum.GetNames(typeof(T));

            for (int i = 0; i < names.Length; i++)
            {
                if (GUILayout.Button(names[i], Instance._styleButton, Instance._options))
                {
                    value = values[i];
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            MarginField();

            return value;
        }
    }
}