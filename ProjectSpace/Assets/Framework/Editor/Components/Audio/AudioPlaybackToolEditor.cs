using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Framework.Runtime.Audio;

namespace Framework.Editor.Audio
{
    /// <summary>
    /// Handles the Playback Tool Editor Window
    /// Can play AudioFileObjects, AudioFileMusicObjects and generic AudioClips
    /// Double click on the former to automatically open the window
    /// </summary>
    public class AudioPlaybackToolEditor : EditorWindow
    {
        static Texture2D cachedTex;
        public static bool forceRepaint;

        static Vector2 dragStartPos = Vector2.zero;
        static bool mouseGrabbed = false;
        static bool mouseDragging = false;
        static bool mouseScrubbed = false;
        static bool loopClip = false;
        static bool clipPlaying = false;
        static bool clipPaused = false;

        public static GameObject helperObject;
        public static AudioSource helperSource;
        public static AudioSourceHelper helperHelper;

        static Color buttonPressedColor = new Color(0.475f, 0.475f, 0.475f);
        static Color buttonPressedColorLighter = new Color(0.75f, 0.75f, 0.75f);

        static float scrollbarProgress = 0;
        static float trueScrollProgress = 0;
        public const float MAX_SCROLL_ZOOM = 50;
        public static float scrollZoom = MAX_SCROLL_ZOOM;

        static Vector2 lastWindowSize = Vector2.zero;
        static bool resized = false;

        static bool showHowTo;
        static Vector2 guideScrollProgress = Vector2.zero;
        static float playbackPreviewClamped = 300;
        static bool showLibraryView = false;

        static AudioPlaybackToolEditor window;
        public static AudioPlaybackToolEditor Window
        {
            get
            {
                if (window == null) window = GetWindow<AudioPlaybackToolEditor>();
                return window;
            }
        }

        public static bool WindowOpen
        {
            get
            {
#if UNITY_2019_4_OR_NEWER
                return HasOpenInstances<AudioPlaybackToolEditor>();
#else
                return window != null;
#endif
            }
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/JSAM/JSAM Playback Tool")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            window = GetWindow<AudioPlaybackToolEditor>();
            window.Show();
            window.Focus();
            window.titleContent.text = "JSAM Playback Tool";
            // Refresh window contents
            window.OnSelectionChange();
            lastWindowSize = Window.position.size;
        }

        [OnOpenAsset]
        public static bool OnDoubleClickAssets(int instanceID, int line)
        {
            string assetPath = AssetDatabase.GetAssetPath(instanceID);
            AudioClipObject audioFile = AssetDatabase.LoadAssetAtPath<AudioClipObject>(assetPath);
            if (audioFile)
            {
                Init();
                // Force a repaint
                Selection.activeObject = null;
                EditorApplication.delayCall += () => Selection.activeObject = audioFile;
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            SetIcons();
        }

        private void OnDisable()
        {
            window = null;

            if (selectedSound)
            {
                Selection.activeObject = null;
                EditorApplication.delayCall += () => Selection.activeObject = selectedSound;
            }
         
            DestroyAudioHelper();
        }

        private void OnGUI()
        {
            if (selectedClip || selectedSound)
            {
                DrawPlaybackTool(selectedClip, selectedSound);
                EditorGUILayout.LabelField("Now Playing - " + helperSource.clip.name);

                if (!resized)
                {
                    if (lastWindowSize != Window.position.size)
                    {
                        lastWindowSize = Window.position.size;
                        resized = true;
                    }
                }
                // if (selectedSound)
                // {
                //     if (selectedSound.useLibrary && selectedSound.GetFileCount() > 1)
                //     {
                //         showLibraryView = EditorCompatability.SpecialFoldouts(showLibraryView, "Show Audio File Object Library");
                //         if (showLibraryView)
                //         {
                //             foreach (AudioClip sound in selectedSound.GetFiles())
                //             {
                //                 Color colorbackup = GUI.backgroundColor;
                //                 //EditorGUILayout.BeginHorizontal();
                //                 if (helperSource.clip == sound) GUI.backgroundColor = buttonPressedColor;
                //                 GUIContent bContent = new GUIContent(sound.name, "Click to change AudioClip being played back to " + sound.name);
                //                 if (GUILayout.Button(bContent))
                //                 {
                //                     // Play the sound
                //                     selectedClip = sound;
                //                     helperSource.clip = selectedClip;
                //                     AudioFileSoundObjectEditor.instance.StartFading(helperSource.clip);
                //                     clipPlaying = true;
                //                 }
                //                 //EditorGUILayout.EndHorizontal();
                //                 GUI.backgroundColor = colorbackup;
                //             }
                //         }
                //         EditorCompatability.EndSpecialFoldoutGroup();
                //     }
                // }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "No JSAM Audio File selected, select one in the Project window to preview it!"
                    , MessageType.Info);
                EditorGUILayout.Space();
            }
        }

        AudioClip selectedClip;
        AudioClipObject selectedSound;

        private void OnSelectionChange()
        {
            if (Selection.activeObject == null) return;
            System.Type activeType = Selection.activeObject.GetType();

            selectedClip = null;
            selectedSound = null;

            if (activeType.Equals(typeof(AudioClip)))
            {
                selectedClip = (AudioClip)Selection.activeObject;
                CreateAudioHelper(selectedClip);
            }
            else if (activeType.Equals(typeof(AudioClipObject)))
            {
                selectedSound = ((AudioClipObject)Selection.activeObject);
                selectedClip = selectedSound.ReferenceAudioClip.editorAsset;
                CreateAudioHelper(selectedClip);
            }
            else
            {
                DoForceRepaint(true);
                return;
            }
            helperSource.clip = selectedClip;

            DoForceRepaint(true);
        }

        /// <summary>
        /// Draws a playback 
        /// </summary>
        /// <param name="music"></param>
        public void DrawPlaybackTool(AudioClip selectedClip, AudioClipObject selectedSound = null)
        {
            Rect progressRect = ProgressBar((float)helperSource.timeSamples / (float)helperSource.clip.samples, selectedClip, selectedSound);
            EditorGUI.BeginChangeCheck();
            scrollbarProgress = GUILayout.HorizontalScrollbar(scrollbarProgress, scrollZoom, 0, MAX_SCROLL_ZOOM);
            if (EditorGUI.EndChangeCheck())
            {
                DoForceRepaint(true);
            }

            EditorGUILayout.BeginHorizontal();

            PollMouseEvents(progressRect);

            if (GUILayout.Button(s_BackIcon, new GUILayoutOption[] { GUILayout.MaxHeight(20) }))
            {
                helperSource.timeSamples = 0;
                helperSource.Stop();
                mouseScrubbed = false;
                clipPaused = false;
                clipPlaying = false;
            }

            // Draw Play Button
            Color colorbackup = GUI.backgroundColor;
            GUIContent buttonIcon = (clipPlaying) ? s_PlayIcons[1] : s_PlayIcons[0];
            if (clipPlaying) GUI.backgroundColor = buttonPressedColor;
            if (GUILayout.Button(buttonIcon, new GUILayoutOption[] { GUILayout.MaxHeight(20) }))
            {
                clipPlaying = !clipPlaying;
                if (clipPlaying)
                {
                    // Note: For some reason, reading from helperSource.time returns 0 even if timeSamples is not 0
                    // However, writing a value to helperSource.time changes timeSamples to the appropriate value just fine
                    if (selectedSound)
                    {
                        helperHelper.Play(selectedSound,0,0);
                        //AudioFileSoundObjectEditor.instance.StartFading(helperSource.clip);
                    }
                    else if (selectedClip)
                    {
                        //helperHelper.PlayDebug(mouseScrubbed);
                    }
                    if (clipPaused) helperSource.Pause();
                }
                else
                {
                    helperSource.Stop();
                    if (!mouseScrubbed)
                    {
                        helperSource.time = 0;
                    }
                    clipPaused = false;
                }
            }

            // Pause button
            GUI.backgroundColor = colorbackup;
            GUIContent theText = (clipPaused) ? s_PauseIcons[1] : s_PauseIcons[0];
            if (clipPaused) GUI.backgroundColor = buttonPressedColor;
            if (GUILayout.Button(theText, new GUILayoutOption[] { GUILayout.MaxHeight(20) }))
            {
                clipPaused = !clipPaused;
                if (clipPaused)
                {
                    helperSource.Pause();
                }
                else
                {
                    helperSource.UnPause();
                }
            }

            // Loop button
            GUI.backgroundColor = colorbackup;
            buttonIcon = (loopClip) ? s_LoopIcons[1] : s_LoopIcons[0];
            if (loopClip) GUI.backgroundColor = buttonPressedColor;
            if (GUILayout.Button(buttonIcon, new GUILayoutOption[] { GUILayout.MaxHeight(20) }))
            {
                loopClip = !loopClip;
                // helperSource.loop = true;
            }
            GUI.backgroundColor = colorbackup;

            if (selectedSound)
            {
                // using (new EditorGUI.DisabledScope(selectedSound.GetFileCount() < 2))
                // {
                //     if (GUILayout.Button(new GUIContent("Play Random", "Preview settings with a random track from your library. Only usable if this Audio File has \"Use Library\" enabled.")))
                //     {
                //         // selectedClip = AudioFileSoundObjectEditor.instance.DesignateRandomAudioClip();
                //         // clipPlaying = true;
                //         // helperSource.Stop();
                //         // AudioFileSoundObjectEditor.instance.StartFading(selectedClip);
                //     }
                // }
            }

            int loopPointInputMode = 0;

            GUIContent blontent = new GUIContent();
            GUIStyle rightJustified = new GUIStyle(EditorStyles.label);
            rightJustified.alignment = TextAnchor.UpperRight;
            EditorGUILayout.LabelField(blontent, rightJustified);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        void PollMouseEvents(Rect progressRect)
        {
            Event evt = Event.current;

            if (evt.isScrollWheel)
            {
                if (evt.mousePosition.y > progressRect.yMin && evt.mousePosition.y < progressRect.yMax)
                {
                    float destinedProgress = Mathf.InverseLerp(progressRect.xMin, progressRect.xMax, evt.mousePosition.x);

                    destinedProgress = Mathf.Lerp(CalculateZoomedLeftValue(), CalculateZoomedRightValue(), destinedProgress);

                    // Center the scrollbar because scrollbars have their pivot set to the left
                    scrollbarProgress = destinedProgress * MAX_SCROLL_ZOOM - scrollZoom / 2;

                    scrollZoom = Mathf.Clamp(scrollZoom + evt.delta.y / 3, 0, MAX_SCROLL_ZOOM);

                    // Because scrollbar progress isn't real progress
                    trueScrollProgress = destinedProgress;
                    DoForceRepaint(true);
                }
            }
            else if (evt.isMouse)
            {
                switch (evt.type)
                {
                    case EventType.MouseUp:
                        switch (evt.button)
                        {
                            case 0:
                                mouseDragging = false;
                                break;
                            case 2:
                                mouseGrabbed = false;
                                break;
                        }
                        break;
                    case EventType.MouseDown:
                    case EventType.MouseDrag:
                        if (evt.button == 0) // Left Mouse Events
                        {
                            if (evt.type == EventType.MouseDown)
                            {
                                // Only begin dragging if mouse is in the waveform window
                                if (evt.mousePosition.y > progressRect.yMin && evt.mousePosition.y < progressRect.yMax)
                                {
                                    mouseDragging = true;
                                    mouseScrubbed = true;
                                }
                                else mouseDragging = false;
                            }
                            if (!mouseDragging) break;
                            float newProgress = Mathf.InverseLerp(progressRect.xMin, progressRect.xMax, evt.mousePosition.x);
                            newProgress = Mathf.Lerp(CalculateZoomedLeftValue(), CalculateZoomedRightValue(), newProgress);
                            helperSource.time = Mathf.Clamp((newProgress * helperSource.clip.length), 0, helperSource.clip.length - 0.000001f);
                        }
                        else if (evt.button == 2) // Middle mouse events
                        {
                            if (!mouseGrabbed)
                            {
                                if (evt.mousePosition.y > progressRect.yMin && evt.mousePosition.y < progressRect.yMax)
                                {
                                    mouseGrabbed = true;
                                    dragStartPos = evt.mousePosition;
                                }
                            }
                            if (mouseGrabbed)
                            {
                                float delta = Mathf.InverseLerp(progressRect.xMin, progressRect.xMax, Mathf.Abs(evt.delta.x));

                                if (evt.delta.x < 0) delta *= -1;
                                
                                scrollbarProgress -= delta * MAX_SCROLL_ZOOM;

                                DoForceRepaint(true);
                            }
                        }
                        break;
                }
            }
        }

        public static void CreateAudioHelper(AudioClip selectedClip, bool designateMusicHelper = false)
        {
            if (helperObject == null)
            {
                helperObject = GameObject.Find("JSAM Audio Helper");
                if (helperObject == null)
                {
                    helperObject = new GameObject("JSAM Audio Helper");
                    helperHelper = helperObject.AddComponent<AudioSourceHelper>();
                    helperSource = helperObject.AddComponent<AudioSource>();
                    helperSource.playOnAwake = false;
                    helperSource.clip = selectedClip;
                    helperSource.time = 0;
                }
                else
                {
                    helperHelper = helperObject.GetComponent<AudioSourceHelper>();
                }
                helperObject.hideFlags = HideFlags.HideAndDontSave;
            }

            if (helperSource == null)
            {
                helperSource = helperObject.GetComponent<AudioSource>();
                helperSource.clip = selectedClip;
            }
        }

        public static void DestroyAudioHelper()
        {
            if (helperSource)
            {
                helperSource.Stop();
            }
            if (!WindowOpen)
            {
                DestroyImmediate(helperObject);
            }
        }

        /// <summary>
        /// Conveniently draws a progress bar
        /// Referenced from the official Unity documentation
        /// https://docs.unity3d.com/ScriptReference/Editor.html
        /// </summary>
        /// <param name="value"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static Rect ProgressBar(float value, AudioClip selectedClip, AudioClipObject selectedSound = null)
        {
            // Get a rect for the progress bar using the same margins as a text field
            // TODO: Make this dynamic, locks its previous size before showing the guide
            float maxHeight = (showHowTo) ? playbackPreviewClamped : 4000;
            float minHeight = (showHowTo) ? playbackPreviewClamped : 64;
            Rect rect = GUILayoutUtility.GetRect(64, 4000, minHeight, maxHeight);

            if (cachedTex == null || forceRepaint)
            {
                Texture2D waveformTexture;
                // if (selectedSound)
                // {
                //     waveformTexture = AudioFileSoundObjectEditor.instance.PaintWaveformSpectrum(helperSource.clip, (int)rect.width, (int)rect.height, new Color(1, 0.5f, 0));
                // }
                // else
                {
                    waveformTexture = PaintWaveformSpectrum(selectedClip, (int)rect.width, (int)rect.height, new Color(1, 0.5f, 0));
                }
                cachedTex = waveformTexture;

                if (waveformTexture != null)
                    GUI.DrawTexture(rect, waveformTexture);
                forceRepaint = false;
            }
            else
            {
                GUI.DrawTexture(rect, cachedTex);
            }

            float left = CalculateZoomedLeftValue();
            float right = CalculateZoomedRightValue();
            if (value >= left && value <= right)
            {
                Rect progressRect = new Rect(rect);
                progressRect.width *= Mathf.InverseLerp(left, right, value);
                progressRect.xMin = progressRect.xMax - 1;
                GUI.Box(progressRect, "", "SelectionRect");
            }

            EditorGUILayout.Space();

            return rect;
        }

        void Update()
        {
            if (selectedClip == null) return;
            if (helperSource == null) CreateAudioHelper(selectedClip);

            if (!helperSource.isPlaying && mouseDragging || resized)
            {
                DoForceRepaint(resized);
            }

            #region Sound Update
            if (selectedSound || (selectedClip))
            {
                if ((clipPlaying && !clipPaused) || (mouseDragging && clipPlaying))
                {
                    float clipPos = helperSource.timeSamples / (float)selectedClip.frequency;
                    // if (selectedSound)
                    // {
                    //     if (selectedSound.fadeMode != FadeMode.None)
                    //     {
                    //         AudioFileSoundObjectEditor.instance.HandleFading(selectedSound);
                    //     }
                    //     else
                    //     {
                    //         helperSource.volume = selectedSound.relativeVolume;
                    //     }
                    // }
                    
                    Repaint();

                    if (!helperSource.isPlaying && !clipPaused && clipPlaying)
                    {
                        helperSource.time = 0;
                        if (loopClip)
                        {
                            helperSource.Play();
                        }
                        else
                        {
                            clipPlaying = false;
                        }
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Code from these gents
        /// https://answers.unity.com/questions/189886/displaying-an-audio-waveform-in-the-editor.html
        /// </summary>
        public static Texture2D PaintWaveformSpectrum(AudioClip audio, int width, int height, Color col)
        {
            if (Event.current.type != EventType.Repaint) return null;

            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float[] samples = new float[audio.samples * audio.channels];
            // Copy sample data to array
            audio.GetData(samples, 0);

            float leftValue = CalculateZoomedLeftValue();
            float rightValue = CalculateZoomedRightValue();

            int leftSide = Mathf.RoundToInt(leftValue * samples.Length);
            int rightSide = Mathf.RoundToInt(rightValue * samples.Length);

            float zoomLevel = scrollZoom / MAX_SCROLL_ZOOM;
            int packSize = Mathf.RoundToInt((int)samples.Length / (int)width * (float)zoomLevel) + 1;

            int s = 0;
            int limit = Mathf.Min(rightSide, samples.Length);

            // Build waveform data
            float[] waveform = new float[limit];
            for (int i = leftSide; i < limit; i += packSize)
            {
                waveform[s] = Mathf.Abs(samples[i]);
                s++;
            }

            Color lightShade = new Color(0.3f, 0.3f, 0.3f);
            int halfHeight = height / 2;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tex.SetPixel(x, y, lightShade);
                }
            }

            for (int x = 0; x < Mathf.Clamp(rightSide, 0, width); x++)
            {
                // Scale the wave vertically relative to half the rect height and the relative volume
                float heightLimit = waveform[x] * halfHeight;

                for (int y = (int)heightLimit; y >= 0; y--)
                {
                    Color currentPixelColour = tex.GetPixel(x, halfHeight + y);
                    if (currentPixelColour == Color.black) continue;

                    tex.SetPixel(x, halfHeight + y, lightShade + col * 0.75f);

                    // Get data from upper half offset by 1 unit due to int truncation
                    tex.SetPixel(x, halfHeight - (y + 1), lightShade + col * 0.75f);
                }
            }

            tex.Apply();

            return tex;
        }

        static GUIContent s_BackIcon = null;
        static GUIContent[] s_PlayIcons = { null, null };
        static GUIContent[] s_PauseIcons = { null, null };
        static GUIContent[] s_LoopIcons = { null, null };

        /// <summary>
        /// Why does Unity keep all this stuff secret?
        /// https://unitylist.com/p/5c3/Unity-editor-icons
        /// </summary>
        static void SetIcons()
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
        }

        public static string TimeToString(float time)
        {
            time *= 1000;
            int minutes = (int)time / 60000;
            int seconds = (int)time / 1000 - 60 * minutes;
            int milliseconds = (int)time - minutes * 60000 - 1000 * seconds;
            return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }

        public static void DoForceRepaint(bool fullRepaint = false)
        {
            forceRepaint = fullRepaint;
            if (WindowOpen)
            {
                resized = false;
                Window.Repaint();
            }
        }

        public static float CalculateZoomedLeftValue()
        {
            return scrollbarProgress / MAX_SCROLL_ZOOM;
        }

        public static float CalculateZoomedRightValue()
        {
            return Mathf.Clamp01((scrollbarProgress + scrollZoom) / MAX_SCROLL_ZOOM);
        }
    }
}