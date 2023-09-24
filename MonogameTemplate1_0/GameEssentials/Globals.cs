using Apos.Shapes;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FontStashSharp;

namespace MonogameTemplate1_0
{
    /// <summary>
    /// Contains a bunch of information relating to drawing and sounds. Acsess using Globals.Inst
    /// </summary>
    public class Globals
    {
        public static Globals Inst;

        public DrawBuffer drawBuffer;

        public SpriteBatch spriteBatch;

        public ShapeBatch postShapeBatch;
        public ShapeBatch shapeBatch;


        private FontSystem _fonts;


        private SpriteFontBase _fontCache;
        public SpriteFontBase Font
        {
            get
            {
                if(_fontCache == null)
                {
                    _fontCache = _fonts.GetFont(36);
                }
                return _fontCache;
            }
        }

        public Music music;
        public Textures textures;
        public Effects effects;
        public SoundFX sounds;

        public GraphicsDevice Graphics;
        public Game Game;
        public Camera Camera;

        private Point _windowSize;
        
        public Point WindowSize
        {
            get
            {
                return _windowSize;
            }
            set
            {
                _windowSize = value;
                if(Camera != null)
                {
                    Camera.ScreenSize = _windowSize;
                }
            }
        }

        public Globals(Game game, Point windowSize)
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            shapeBatch = new ShapeBatch(game.GraphicsDevice, game.Content);
            postShapeBatch = new ShapeBatch(game.GraphicsDevice, game.Content);

            Graphics = game.GraphicsDevice;

            this.Game = game;

            _fonts = new FontSystem();

            Camera = new Camera(windowSize);
            this.WindowSize = windowSize;

            effects = new Effects();
            textures = new Textures();
            music = new Music();
        }

        /// <summary>
        /// Loads a SoundEffect into the SoundFX class. Use a filename, eg. explosion, NOT explosion.wav
        /// </summary>
        public void AddSound(string name)
        {
            sounds.addSound(name, Game.Content.Load<SoundEffect>(name));
        }

        /// <summary>
        /// Loads a group of sound effects into the SoundFX class under the same name. Count indicates number of sounds to be loaded. Sounds in the folder must be name like: bundleName1, bundleName2, ect.
        /// </summary>
        public void AddSound(string bundleName, int count)
        {
            SoundEffect[] sounds = new SoundEffect[count];

            for (int i = 0; i < count; i++)
            {
                string file = string.Format($"{bundleName}/{bundleName}{i + 1}");
                sounds[i] = Game.Content.Load<SoundEffect>(file);
            }
            this.sounds.addSound(bundleName, sounds);
        }

        /// <summary>
        /// Loads a Song into the Music class. Use a filename, eg. rickRoll, NOT rockRoll.mp3
        /// </summary>
        public void AddMusic(string name)
        {
            music.AddSong(name, Game.Content.Load<Song>(name));
        }

        /// <summary>
        /// Loads a texture into the Textures class. Use a filename, eg. car, NOT car.png
        /// </summary>
        public void AddTexture(string name)
        {
            textures.addTexture(name, Game.Content.Load<Texture2D>(name));
        }

        /// <summary>
        /// Loads an effect into the Effect class. Use a filename, eg. bloom, NOT bloom.fx
        /// </summary>
        public void AddEffect(string name)
        {
            effects.addEffect(name, Game.Content.Load<SpriteEffect>(name));
        }

        /// <summary>
        /// Sets the font of the globals class
        /// </summary>
        public void SetFont(string name)
        {
            _fonts.AddFont(File.ReadAllBytes("Content/" + name + ".ttf"));
            drawBuffer = new DrawBuffer(spriteBatch);
        }

        /// <summary>
        /// Allows for the easy loading and usage of SoundEffects in Monogame (.wav)
        /// </summary>
        public class SoundFX
        {
            private Dictionary<string, SoundEffect[]> soundDictionary = new Dictionary<string, SoundEffect[]>();
            private float volume = 1;

            public SoundFX(){}

            public void addSound(string name, SoundEffect sound)
            {
                soundDictionary[name] = new SoundEffect[1] { sound };
            }

            public void addSound(string bundleName, params SoundEffect[] sounds)
            {
                soundDictionary[bundleName] = sounds;
            }

            public void play(string name)
            {
                if (soundDictionary.TryGetValue(name, out SoundEffect[] soundBundle))
                {
                    int index = Random.Shared.Next(soundBundle.Length);
                    SoundEffectInstance instance = soundBundle[index].CreateInstance();
                    instance.Volume = volume;
                    instance.Play();
                }
                else
                {
                    throw new Exception($"Sound File: {name} does not exist");
                }
            }

            public void play(string name, float? volume, float? pan, float pitch = 1)
            {
                if (soundDictionary.TryGetValue(name, out SoundEffect[] soundBundle))
                {
                    int index = Random.Shared.Next(soundBundle.Length);

                    if (pan.HasValue)
                    {
                        float value = MathF.Atan(-pan.Value / 240) * 2 / MathF.PI;
                        playEffect(true, (volume.HasValue ? volume.Value : 1) * linearPan(value), pitch, soundBundle[index]);
                        playEffect(false, (volume.HasValue ? volume.Value : 1) * linearPan(-value), pitch, soundBundle[index]);
                    }
                    else
                    {
                        SoundEffectInstance instance = soundBundle[index].CreateInstance();
                        if (volume.HasValue)
                            instance.Volume = MathHelper.Clamp(volume.Value, 0f, 1f) * this.volume;

                        instance.Pitch = pitch;

                        instance.Play();
                    }
                }
                else
                {
                    throw new Exception($"Sound File: {name} does not exist");
                }
            }

            private float linearPan(float pan)
            {
                return 0.5f * pan + 0.5f;
            }

            private void playEffect(bool right, float volume, float pitch, SoundEffect sound)
            {
                SoundEffectInstance instance = sound.CreateInstance();
                if (right)
                {
                    instance.Pan = 1;
                }
                else
                {
                    instance.Pan = -1;
                }

                instance.Volume = volume * this.volume;

                instance.Pitch = pitch;
                instance.Play();
            }

            public void SetVolume(float value)
            {
                this.volume = MathHelper.Clamp(value, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// Allows for the easy loading and usage of Songs in Monogame (.mp3)
        /// </summary>
        public class Music
        {
            private Dictionary<string, Song> songDictionary = new Dictionary<string, Song>();
            private Song currentSong;
            private bool isLooping = false;

            public float volumeFactor = 1;

            List<Animation> animations = new List<Animation>();

            float volume;
            public Music()
            {

            }

            public void AddSong(string name, Song song)
            {
                songDictionary[name] = song;
            }

            public void Play(string name, bool loop)
            {
                if (currentSong != null)
                {
                    CrossfadeToSong(name, loop);
                    return;
                }

                if (songDictionary.TryGetValue(name, out var song))
                {
                    isLooping = loop;
                    MediaPlayer.IsRepeating = isLooping;
                    MediaPlayer.Play(song);
                }
                else
                {
                    throw new Exception($"Song File: {name} does not exist");
                }
            }

            private void CrossfadeToSong(string nextSongName, bool loop)
            {
                if (songDictionary.TryGetValue(nextSongName, out var nextSong))
                {
                    var fadeOut = new Animation(24, AnimationType.sigmoid, MediaPlayer.Volume, 0, (animationValue) =>
                    {
                        MediaPlayer.Volume = animationValue;
                    });

                    fadeOut.onEnd = () =>
                    {
                        MediaPlayer.Stop();
                        MediaPlayer.Volume = volume;
                        MediaPlayer.Play(nextSong);

                        var fadeIn = new Animation(24, AnimationType.inverseParabolic, 0, volume, (animationValue) =>
                        {
                            MediaPlayer.Volume = animationValue;
                        });

                        fadeIn.onEnd = () =>
                        {
                            currentSong = nextSong;

                            isLooping = loop;
                            MediaPlayer.IsRepeating = isLooping;
                        };

                        animations.Add(fadeIn);
                    };

                    animations.Add(fadeOut);
                }
                else
                {
                    throw new Exception($"Song File: {nextSongName} does not exist");
                }
            }

            public void Pause()
            {
                Animation fadeOut = new Animation(24, AnimationType.sigmoid, MediaPlayer.Volume, 0, (animationValue) => MediaPlayer.Volume = animationValue);
                fadeOut.onEnd = () => MediaPlayer.Pause();
                animations.Add(fadeOut);
            }

            public void Pause(int fadeLength)
            {
                Animation fadeOut = new Animation(fadeLength, AnimationType.sigmoid, MediaPlayer.Volume, 0, (animationValue) => MediaPlayer.Volume = animationValue);
                fadeOut.onEnd = () => MediaPlayer.Pause();
                animations.Add(fadeOut);
            }

            public void Resume()
            {
                MediaPlayer.Resume();
                Animation fadeIn = new Animation(24, AnimationType.sigmoid, 0, volume, (animationValue) => MediaPlayer.Volume = animationValue);
                animations.Add(fadeIn);
            }

            public void Stop()
            {
                MediaPlayer.Stop();
                isLooping = false;
                MediaPlayer.IsRepeating = false;
            }

            public void SetVolume(float volume)
            {
                MediaPlayer.Volume = MathHelper.Clamp(volume, 0.0f, 1.0f);
                this.volume = volume;
            }

            public bool IsPlaying()
            {
                return MediaPlayer.State == MediaState.Playing;
            }

            public void tick(double deltaTime)
            {
                for (int i = 0; i < animations.Count; i++)
                {
                    animations[i].Tick(deltaTime);
                }
            }
        }

        /// <summary>
        /// Allows for the easy loading and usage of texture
        /// </summary>
        public class Textures
        {
            Dictionary<string, Texture2D> textureDictionary = new Dictionary<string, Texture2D>();
            public Textures()
            {
            }

            public void addTexture(string name, Texture2D texture)
            {
                textureDictionary[name] = texture;
            }

            public Texture2D get(string name)
            {
                if (textureDictionary.TryGetValue(name, out Texture2D texture))
                {
                    return texture;
                }
                else
                {
                    throw new Exception($"Texture2D File: {name} does not exist");
                }
            }
        }

        /// <summary>
        /// Allows for the easy loading and usage of effects
        /// </summary>
        public class Effects
        {
            Dictionary<string, SpriteEffect> textureDictionary = new Dictionary<string, SpriteEffect>();
            public Effects()
            {
            }

            public void addEffect(string name, SpriteEffect texture)
            {
                textureDictionary[name] = texture;
            }

            public SpriteEffect get(string name)
            {
                if (textureDictionary.TryGetValue(name, out SpriteEffect texture))
                {
                    return texture;
                }
                else
                {
                    throw new Exception($"SpriteEffect File: {name} does not exist");
                }
            }
        }
    }

    /// <summary>
    /// Simple Camera class with location, zoom, and screen shake.
    /// </summary>
    public class Camera
    {
        public float screenShake
        {
            private get;
            set;
        }

        private Vector2 _screenShake = Vector2.Zero;
        private float globalZoomFactor;
        private float _zoom;
        public float Zoom
        {
            get
            {
                return _zoom * globalZoomFactor;
            }
            private set
            {
                _zoom = value;
            }
        }

        private Vector2 _position;
        public Vector2 Position
        {
            get
            {
                return _position + _screenShake;
            }
            private set
            {
                _position = value;
            }
        }
        private Point _screenSize;

        public Point ScreenSize
        {
            get
            {
                return _screenSize;
            }
            set
            {
                _screenSize = value;
                globalZoomFactor = _screenSize.ToVector2().LengthSquared() / (1920 * 1920 + 1080 * 1080);
                ScreenSizeVectorHalf = _screenSize.ToVector2() / 2f;
            }
        }


        private Vector2 ScreenSizeVectorHalf;



        public Camera(Point screenSize)
        {
            Zoom = 1;
            Position = screenSize.ToVector2() / 2;
            ScreenSize = screenSize;
        }

        public Vector2 ScreenToWorld(Vector2 screen)
        {
            return (screen - _screenSize.ToVector2() / 2) / Zoom + Position;
        }

        public Vector2 WorldToScreen(Vector2 world)
        {
            return (world - Position) * Zoom + ScreenSizeVectorHalf;
        }

        public void Translate(Vector2 offset)
        {
            Position -= offset;
        }
        public void SetLocation(Vector2 loc)
        {
            Position = loc;
        }

        public void SetZoom(float zoom)
        {
            Zoom = Math.Max(zoom, 0.00001f);
        }
        public void ChangeZoom(float zoom)
        {
            Zoom *= zoom;
            Zoom = Math.Max(Zoom, 0.00001f);
        }

        public void update(double deltaTime)
        {
            if (screenShake > 0)
            {
                screenShake = Math.Max(screenShake - (float)deltaTime, 0);
                const int screenShakeRange = 16;
                _screenShake = new Vector2(Random.Shared.Next(screenShakeRange) - screenShakeRange / 2, Random.Shared.Next(screenShakeRange) - screenShakeRange / 2);
            }
            else
            {
                _screenShake = Vector2.Zero;
            }
        }
    }

    /// <summary>
    /// Static class containing many helpful functions, mainly related to vectors (Also Check MathHelper.*)
    /// </summary>
    public static class MathFunc
    {
        public static float Normailise(float max, float min, float value)
        {
            return (Math.Max(min, Math.Min(max, value)) - min) / (max - min);
        }

        public static Vector2 UnitVectorPoint(Vector2 A, Vector2 B)
        {
            return Vector2.Normalize(B - A);
        }
        public static float VectorPointAngle(Vector2 PointFrom, Vector2 PointTo)
        {
            return (MathHelper.ToDegrees(MathF.Atan2(PointTo.Y - PointFrom.Y, PointTo.X - PointFrom.X)) + 360) % 360;
        }

        public static Vector2 SetVectorAngle(float rotation, Vector2 vector)
        {
            float magnitude = vector.Length();
            float radians = (float)(rotation * MathF.PI / 180);
            float x = magnitude * (float)MathF.Cos(radians);
            float y = magnitude * (float)MathF.Sin(radians);
            return new Vector2(x, y);
        }

        public static Vector2 RotateVector(Vector2 vector, float angle)
        {
            float angleInRadians = MathHelper.ToRadians(angle);
            float cos = (float)Math.Cos(angleInRadians);
            float sin = (float)Math.Sin(angleInRadians);

            float newX = vector.X * cos - vector.Y * sin;
            float newY = vector.X * sin + vector.Y * cos;

            return new Vector2(newX, newY);
        }

        public static Vector2 RotateVectorOrigin(Vector2 vector, float angle, Vector2 origin)
        {
            Vector2 translatedVector = vector - origin;

            float angleInRadians = MathHelper.ToRadians(angle);
            float cos = (float)Math.Cos(angleInRadians);
            float sin = (float)Math.Sin(angleInRadians);

            float newX = translatedVector.X * cos - translatedVector.Y * sin;
            float newY = translatedVector.X * sin + translatedVector.Y * cos;

            Vector2 rotatedVector = new Vector2(newX, newY) + origin;

            return rotatedVector;
        }

        public static bool DoLinesIntersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
        {
            float denominator = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);

            if (denominator == 0)
            {
                return false;
            }

            float numerator1 = (A.Y - C.Y) * (D.X - C.X) - (A.X - C.X) * (D.Y - C.Y);
            float numerator2 = (A.Y - C.Y) * (B.X - A.X) - (A.X - C.X) * (B.Y - A.Y);

            float t1 = numerator1 / denominator;
            float t2 = numerator2 / denominator;

            if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
            {
                return true;
            }

            return false;
        }

        public static Vector2 SetDistance(float newDistance, Vector2 vector)
        {
            float currentAngle = MathF.Atan2(vector.Y, vector.X);
            return new Vector2(newDistance * MathF.Cos(currentAngle), newDistance * MathF.Sin(currentAngle));
        }
        public static float AngleBetweenVectors(Vector2 vector1, Vector2 vector2)
        {
            float angleA = MathFunc.GetAngle(vector1) + 720;
            float angleB = MathFunc.GetAngle(vector2) + 720;

            float difference = angleA - angleB;
            if (Math.Abs(difference) > 180)
            {
                difference = angleB - angleA;
            }
            return difference;
        }
        public static Vector2 NormalisedNormalVector(Vector2 point1, Vector2 point2)
        {
            Vector2 direction = point2 - point1;

            Vector2 normalVector = new Vector2(-direction.Y, direction.X);

            return Vector2.Normalize(normalVector);
        }

        public static bool RandomProb(float probability, Random random)
        {
            return random.NextDouble() < probability;
        }
        public static float GetAngle(Vector2 value)
        {
            return (float)(MathF.Atan2(value.Y, value.X) * 180 / MathF.PI);
        }
    }

    /// <summary>
    /// Stores DrawCalls to be used later
    /// </summary>
    public class DrawBuffer
    {
        SpriteBatch spriteBatch;

        List<DrawCall>[] drawCalls = new List<DrawCall>[totalDraws];

        public DrawBuffer(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;

            for (int i = 0; i < totalDraws; i++)
            {
                drawCalls[i] = new List<DrawCall>();
            }
        }

        public void AddText(string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, int layer = 0)
        {
            drawCalls[layer].Add(new DrawCall(text, null, position, null, color, rotation, SpriteEffects.None, origin, scale));
        }

        public void AddDraw(Texture2D texture2D, Vector2 position, Color color, int layer = 0)
        {
            drawCalls[layer].Add(new DrawCall(null, texture2D, position, null, color, 0, SpriteEffects.None, Vector2.Zero, 1));
        }
        public void AddDraw(Texture2D texture2D, Rectangle desination, Color color, int layer = 0)
        {
            drawCalls[layer].Add(new DrawCall(null, texture2D, Vector2.Zero, desination, color, 0, SpriteEffects.None, Vector2.Zero, 1));
        }

        public void AddDraw(Texture2D texture2D, Rectangle desination, Color color, Vector2 origin, float rotation, float scale, int layer = 0)
        {
            drawCalls[layer].Add(new DrawCall(null, texture2D, Vector2.Zero, desination, color, rotation, SpriteEffects.None, origin, scale));
        }

        const int totalDraws = 12;

        public void Draw(int min = 0, int max = totalDraws)
        {
            spriteBatch.Begin();
            for (int i = min; i < max; i++)
            {
                foreach (DrawCall draw in drawCalls[i])
                {
                    draw.Draw(spriteBatch, Globals.Inst.Font);
                }

                if (i == totalDraws - 1)
                {
                    for (int j = 0; j < totalDraws; j++)
                    {
                        drawCalls[j].Clear();
                    }
                }
            }
            spriteBatch.End();
        }
    }

    /// <summary>
    /// Simple Tweening Class, interpolates between two floating point values. 
    /// <br>Contains an optional onEnd Action</br>
    /// </summary>
    public class Animation
    {
        private Action<float> value;
        private List<AnimationType> animationTypes = new List<AnimationType>();
        private List<int> totalMilliseconds = new List<int>();
        private float t;
        private float startValue;
        private List<float> endValues = new List<float>();

        public Action onEnd;

        bool enabled = true;

        public bool Delete
        {
            get { return enabled; }
        }

        /// <summary>
        /// Parameter ticks is the length of time in 1/60 second increments. 1 second animation = 60 ticks
        /// </summary>
        public Animation(int ticks, AnimationType type, float start, float end, Action<float> modifier)
        {
            t = 0;
            this.startValue = start;
            this.endValues.Add(end);
            animationTypes.Add(type);
            totalMilliseconds.Add((int)(ticks * (100.0 / 6.0)));
            this.value = modifier;
        }

        public void Tick(double deltaTime)
        {
            if (!enabled)
            {
                return;
            }
            deltaTime *= (100.0 / 6.0);
            if (t < 1.0f)
            {
                t += (float)(deltaTime / totalMilliseconds[0]);
                if (animationTypes[0] != AnimationType.none)
                {
                    float animatedValue = ComputeAnimatedValue();
                    value(animatedValue);
                }
            }
            else
            {
                if (endValues.Count > 1)
                {
                    startValue = endValues[0];
                    endValues.RemoveAt(0);
                    totalMilliseconds.RemoveAt(0);
                    animationTypes.RemoveAt(0);
                    t = 0;
                }
                else
                {
                    if (onEnd != null)
                    {
                        onEnd();
                    }
                    enabled = false;
                }
            }
        }

        private float ComputeAnimatedValue()
        {
            switch (animationTypes[0])
            {
                case AnimationType.linear:
                    return MathHelper.Lerp(startValue, endValues[0], t);
                case AnimationType.parabolic:
                    return ParabolicInterpolation(t);
                case AnimationType.cubic:
                    return CubicInterpolation(t);
                case AnimationType.inverseCubic:
                    return InverseCubicInterpolation(t);
                case AnimationType.sigmoid:
                    return SigmoidInterpolation(t);
                case AnimationType.inverseParabolic:
                    return InverseParabolicInterpolation(t);
                default:
                    return MathHelper.Lerp(startValue, endValues[0], t);
            }
        }

        /// <summary>
        /// Adds another keyframe for the animation
        /// </summary>
        public void AddKeyframe(float endValue, int ticks, AnimationType type)
        {
            this.endValues.Add(endValue);
            this.totalMilliseconds.Add((int)(ticks * (100.0 / 6.0)));
            this.animationTypes.Add(type);
        }

        /// <summary>
        /// Allows you to change the desination of the animation dynamically
        /// </summary>
        public void ChangeEndValue(float value)
        {
            this.endValues[0] = value;
        }

        private float ParabolicInterpolation(float t)
        {
            float tSquared = t * t;
            return tSquared * (endValues[0] - startValue) + startValue;
        }

        private float InverseParabolicInterpolation(float t)
        {
            float tSquared = 1 - (1 - t) * (1 - t);
            return tSquared * (endValues[0] - startValue) + startValue;
        }

        private float CubicInterpolation(float t)
        {
            float tCubed = t * t * t;
            return tCubed * (endValues[0] - startValue) + startValue;
        }

        private float InverseCubicInterpolation(float t)
        {
            float tSquared = 1 - (1 - t) * (1 - t) * (1 - t);
            return tSquared * (endValues[0] - startValue) + startValue;
        }

        private float SigmoidInterpolation(float t)
        {
            float tSquared = (float)(1 / (1 + Math.Exp(10 * (-t + 0.5f))));
            return tSquared * (endValues[0] - startValue) + startValue;
        }
    }

    /// <summary>
    /// Used to detemines the animation style
    /// </summary>
    public enum AnimationType
    {
        linear = 0,
        parabolic = 1,
        cubic = 2,
        inverseCubic = 3,
        sigmoid = 4,
        inverseParabolic = 5, 
        none = 6,
    }

    /// <summary>
    /// Used to store all information needed for a draw call in a spritebatch
    /// </summary>
    public struct DrawCall
    {
        public string text;
        public Texture2D texture;
        public Vector2 position = Vector2.Zero;
        public Rectangle? destination = null;
        public Color color = Color.White;
        public float rotation = 0;
        public SpriteEffects effect = SpriteEffects.None;
        public Vector2 origin = Vector2.Zero;
        public float scale = 1;

        public DrawCall(string text, Texture2D texture, Vector2 position, Rectangle? destination, Color color, float rotation, SpriteEffects effect, Vector2 origin, float scale)
        {
            this.text = text;
            this.texture = texture;
            this.position = position;
            this.destination = destination;
            this.color = color;
            this.rotation = rotation;
            this.effect = effect;
            this.origin = origin;
            this.scale = scale;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFontBase font)
        {
            if (text == null)
            {
                if (destination.HasValue)
                {
                    spriteBatch.Draw(texture, destination.Value, null, color, rotation, origin, effect, 1);
                }
                else
                {
                    spriteBatch.Draw(texture, position, color);
                }
            }
            else
            {
                spriteBatch.DrawString(font, text, position, color, scale: font.MeasureString(text) * scale, origin: origin);
            }
        }
    }
}
