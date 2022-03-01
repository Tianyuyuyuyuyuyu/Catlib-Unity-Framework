using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Framework.Runtime.Audio;

namespace Framework.Editor.Audio
{
    [CustomEditor(typeof(AudioClipObject))]
    public class AudioClipObjectEditor : OdinEditor
    {
        AudioClipObject myScript;

        protected override void OnEnable()
        {
            base.OnEnable();

            myScript = target as AudioClipObject;
            AudioPlaybackToolEditor.CreateAudioHelper(myScript.ReferenceAudioClip.editorAsset, true);
            SetupIcons();
            
            EditorApplication.update += Update;
        }
        
        void OnDisable()
        {
            base.OnDisable();
            EditorApplication.update -= Update;
            AudioPlaybackToolEditor.DestroyAudioHelper();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            DrawPlaybackTool(target as AudioClipObject);
            Repaint();
        }

        public void Update()
        {
            if (!AudioPlaybackToolEditor.helperSource.isPlaying && !clipPaused && clipPlaying)
            {
                AudioPlaybackToolEditor.helperSource.time = 0;
                if (loopClip)
                {
                    AudioPlaybackToolEditor.helperSource.Play();
                }
                else
                {
                    clipPlaying = false;
                }
            }
        }


        /// <summary>
        /// True so long as the inspector music player hasn't looped
        /// </summary>
        public static bool firstPlayback = true;
        public static bool freePlay = false;
        bool loopClip = false;
        Color buttonPressedColor = new Color(0.475f, 0.475f, 0.475f);
        Color buttonPressedColorLighter = new Color(0.75f, 0.75f, 0.75f);
        bool clipPlaying = false;
        bool clipPaused = false;
        bool mouseDragging = false;
        bool mouseScrubbed = false;

        /// <summary>
        /// Draws a playback 
        /// </summary>
        public void DrawPlaybackTool(AudioClipObject myScript)
        {
            GUIContent blontent = new GUIContent("Audio Playback Preview",
                "Allows you to preview how your AudioFileMusicObject will sound during runtime right here in the inspector. " +
                "Some effects, like spatialization, will not be available to preview");

            {
                AudioClip music = myScript.ReferenceAudioClip.editorAsset;
                if(music == null)
                    return;
                Rect progressRect = ProgressBar((float) AudioPlaybackToolEditor.helperSource.timeSamples / (float) AudioPlaybackToolEditor.helperSource.clip.samples, GetInfoString());

                EditorGUILayout.BeginHorizontal();

                Event evt = Event.current;

                if (evt.isMouse)
                {
                    switch (evt.type)
                    {
                        case EventType.MouseUp:
                            mouseDragging = false;
                            break;
                        case EventType.MouseDown:
                        case EventType.MouseDrag:
                            if (evt.type == EventType.MouseDown)
                            {
                                if (evt.mousePosition.y > progressRect.yMin && evt.mousePosition.y < progressRect.yMax)
                                {
                                    mouseDragging = true;
                                    mouseScrubbed = true;
                                }
                                else mouseDragging = false;
                            }

                            if (!mouseDragging) break;
                            float newProgress = Mathf.InverseLerp(progressRect.xMin, progressRect.xMax, evt.mousePosition.x);
                            AudioPlaybackToolEditor.helperSource.time = Mathf.Clamp((newProgress * music.length), 0, music.length - 0.00000001f);
                            break;
                    }
                }

                if (GUILayout.Button(s_BackIcon, new GUILayoutOption[] {GUILayout.MaxHeight(20)}))
                {
                    AudioPlaybackToolEditor.helperSource.timeSamples = 0;

                    AudioPlaybackToolEditor.helperSource.Stop();
                    mouseScrubbed = false;
                    clipPaused = false;
                    clipPlaying = false;
                }

                Color colorbackup = GUI.backgroundColor;
                GUIContent buttonIcon = (clipPlaying) ? s_PlayIcons[1] : s_PlayIcons[0];
                if (clipPlaying) GUI.backgroundColor = buttonPressedColor;
                if (GUILayout.Button(buttonIcon, new GUILayoutOption[] {GUILayout.MaxHeight(20)}))
                {
                    clipPlaying = !clipPlaying;
                    if (clipPlaying)
                    {
                        // Note: For some reason, reading from AudioPlaybackToolEditor.helperSource.time returns 0 even if timeSamples is not 0
                        // However, writing a value to AudioPlaybackToolEditor.helperSource.time changes timeSamples to the appropriate value just fine
                        AudioPlaybackToolEditor.helperHelper.PlayDebug(myScript, mouseScrubbed);
                        if (clipPaused) AudioPlaybackToolEditor.helperSource.Pause();
                        firstPlayback = true;
                        freePlay = false;
                    }
                    else
                    {
                        AudioPlaybackToolEditor.helperSource.Stop();
                        if (!mouseScrubbed)
                        {
                            AudioPlaybackToolEditor.helperSource.time = 0;
                        }

                        clipPaused = false;
                    }
                }

                GUI.backgroundColor = colorbackup;
                GUIContent theText = (clipPaused) ? s_PauseIcons[1] : s_PauseIcons[0];
                if (clipPaused) GUI.backgroundColor = buttonPressedColor;
                if (GUILayout.Button(theText, new GUILayoutOption[] {GUILayout.MaxHeight(20)}))
                {
                    clipPaused = !clipPaused;
                    if (clipPaused)
                    {
                        AudioPlaybackToolEditor.helperSource.Pause();
                    }
                    else
                    {
                        AudioPlaybackToolEditor.helperSource.UnPause();
                    }
                }

                GUI.backgroundColor = colorbackup;
                buttonIcon = (loopClip) ? s_LoopIcons[1] : s_LoopIcons[0];
                if (loopClip) GUI.backgroundColor = buttonPressedColor;
                if (GUILayout.Button(buttonIcon, new GUILayoutOption[] {GUILayout.MaxHeight(20)}))
                {
                    loopClip = !loopClip;
                    // AudioPlaybackToolEditor.helperSource.loop = true;
                }

                GUI.backgroundColor = colorbackup;

                // if (GUILayout.Button(openIcon, new GUILayoutOption[] {GUILayout.MaxHeight(19)}))
                // {
                //     AudioPlaybackToolEditor.Init();
                // }

                GUIStyle rightJustified = new GUIStyle(EditorStyles.label);
                rightJustified.alignment = TextAnchor.UpperRight;
                EditorGUILayout.LabelField(blontent, rightJustified);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
        }
        
        
        Texture2D cachedTex;

        /// <summary>
        /// Conveniently draws a progress bar
        /// Referenced from the official Unity documentation
        /// https://docs.unity3d.com/ScriptReference/Editor.html
        /// </summary>
        /// <param name="value"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        Rect ProgressBar(float value, string label)
        {
            // Get a rect for the progress bar using the same margins as a text field
            Rect rect = GUILayoutUtility.GetRect(64, 64, "TextField");

            AudioClip music = ((AudioClipObject) target).ReferenceAudioClip.editorAsset;

            if (cachedTex == null || AudioPlaybackToolEditor.forceRepaint)
            {
                Texture2D waveformTexture = PaintWaveformSpectrum(music, (int) rect.width, (int) rect.height, new Color(1, 0.5f, 0));
                cachedTex = waveformTexture;
                if (waveformTexture != null)
                    GUI.DrawTexture(rect, waveformTexture);
                AudioPlaybackToolEditor.forceRepaint = false;
            }
            else
            {
                GUI.DrawTexture(rect, cachedTex);
            }

            Rect progressRect = new Rect(rect);
            progressRect.width *= value;
            progressRect.xMin = progressRect.xMax - 1;
            GUI.Box(progressRect, "", "SelectionRect");

            EditorGUILayout.Space();

            return rect;
        }

        /// <summary>
        /// Code from these gents
        /// https://answers.unity.com/questions/189886/displaying-an-audio-waveform-in-the-editor.html
        /// </summary>
        public Texture2D PaintWaveformSpectrum(AudioClip audio, int width, int height, Color col)
        {
            if (Event.current.type != EventType.Repaint) return null;

            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float[] samples = new float[audio.samples * audio.channels];
            // Copy sample data to array
            audio.GetData(samples, 0);

            Color lightShade = new Color(0.3f, 0.3f, 0.3f);
            int halfHeight = height / 2;

            float leftValue = AudioPlaybackToolEditor.CalculateZoomedLeftValue();
            float rightValue = AudioPlaybackToolEditor.CalculateZoomedRightValue();

            int leftSide = Mathf.RoundToInt(leftValue * samples.Length);
            int rightSide = Mathf.RoundToInt(rightValue * samples.Length);

            float zoomLevel = AudioPlaybackToolEditor.scrollZoom / AudioPlaybackToolEditor.MAX_SCROLL_ZOOM;
            int packSize = Mathf.RoundToInt((int) samples.Length / (int) width * (float) zoomLevel) + 1;

            int s = 0;
            int limit = Mathf.Min(rightSide, samples.Length);

            // Build waveform data
            float[] waveform = new float[limit];
            for (int i = leftSide; i < limit; i += packSize)
            {
                waveform[s] = Mathf.Abs(samples[i]);
                s++;
            }
            
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        tex.SetPixel(x, y, lightShade);
                    }
                }
            }

            for (int x = 0; x < Mathf.Clamp(rightSide, 0, width); x++)
            {
                // Scale the wave vertically relative to half the rect height and the relative volume
                float heightLimit = waveform[x] * halfHeight; //* myScript.relativeVolume;

                for (int y = (int) heightLimit; y >= 0; y--)
                {
                    Color currentPixelColour = tex.GetPixel(x, halfHeight + y);

                    tex.SetPixel(x, halfHeight + y, currentPixelColour + col * 0.75f);

                    // Get data from upper half offset by 1 unit due to int truncation
                    currentPixelColour = tex.GetPixel(x, halfHeight - (y + 1));
                    // Draw bottom half with data from upper half
                    tex.SetPixel(x, halfHeight - (y + 1), currentPixelColour + col * 0.75f);
                }
            }

            tex.Apply();

            return tex;
        }

        static GUIContent s_BackIcon = null;
        static GUIContent[] s_PlayIcons = {null, null};
        static GUIContent[] s_PauseIcons = {null, null};
        static GUIContent[] s_LoopIcons = {null, null};
        static GUIContent openIcon;

        /// <summary>
        /// Why does Unity keep all this stuff secret?
        /// https://unitylist.com/p/5c3/Unity-editor-icons
        /// </summary>
        static void SetupIcons()
        {
            s_BackIcon = EditorGUIUtility.TrIconContent("beginButton", "Click to Reset Playback Position");
#if UNITY_2019_4_OR_NEWER
            s_PlayIcons[0] = EditorGUIUtility.TrIconContent("d_PlayButton", "Click to Play");
            s_PlayIcons[1] = EditorGUIUtility.TrIconContent("d_PlayButton On", "Click to Stop");
#else
            s_PlayIcons[0] = EditorGUIUtility.TrIconContent("preAudioPlayOff", "Click to Play");
            s_PlayIcons[1] = EditorGUIUtility.TrIconContent("preAudioPlayOn", "Click to Stop");
#endif
            s_PauseIcons[0] = EditorGUIUtility.TrIconContent("PauseButton", "Click to Pause");
            s_PauseIcons[1] = EditorGUIUtility.TrIconContent("PauseButton On", "Click to Unpause");
#if UNITY_2019_4_OR_NEWER
            s_LoopIcons[0] = EditorGUIUtility.TrIconContent("d_preAudioLoopOff", "Click to enable looping");
            s_LoopIcons[1] = EditorGUIUtility.TrIconContent("preAudioLoopOff", "Click to disable looping");
#else
            s_LoopIcons[0] = EditorGUIUtility.TrIconContent("playLoopOff", "Click to enable looping");
            s_LoopIcons[1] = EditorGUIUtility.TrIconContent("playLoopOn", "Click to disable looping");
#endif
            openIcon = EditorGUIUtility.TrIconContent("d_ScaleTool", "Click to open Playback Preview in a standalone window");
        }
    }
}