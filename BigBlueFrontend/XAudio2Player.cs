using System;
using System.Collections.Generic;
using System.IO;
using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace BigBlue
{
    internal static class XAudio2Player
    {
        public static bool Disabled { get; set; }

        public static XAudio2 xaudio2;
        private static MasteringVoice masteringVoice;

        private static int speakerChannels;

        private static Dictionary<string, float[]> speakerDictionary = new Dictionary<string, float[]>(4);

        internal static Dictionary<string, Sound> LoadedSounds = new Dictionary<string, Sound>();

        internal class Sound
        {
            public SourceVoice Voice { get; set; }

            public AudioBufferAndMetaData AudioBuffer { get; set; }

            public bool Resume { get; set; }
        }

        public sealed class AudioBufferAndMetaData : AudioBuffer
        {
            public WaveFormat WaveFormat { get; set; }
            public uint[] DecodedPacketsInfo { get; set; }
        }
        
        public static void StopAudioEngine()
        {
            if (!Disabled)
            {
                xaudio2.StopEngine();
            }
        }

        public static void RestartAudioEngine()
        {
            if (!Disabled)
            {
                xaudio2.StartEngine();
            }
        }

        private static void Xaudio2_CriticalError1(object sender, SharpDX.XAudio2.ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        public enum AudioPlayerState
        {
            /// <summary>
            /// The player is stopped (default).
            /// </summary>
            Stopped,

            /// <summary>
            /// The player is playing a sound.
            /// </summary>
            Playing,

            /// <summary>
            /// The player is paused.
            /// </summary>
            Paused,
        }

        /// <summary>
        /// Gets the state of this instance.
        /// </summary>
        /// <value>The state.</value>
        public static AudioPlayerState State { get; private set; }

        public static void ProvisionPlayer(float volume)
        {
            try
            {
                // if the volume is 0, why even bother loading it?
                if (volume > 0f)
                {
                    xaudio2 = new XAudio2();

                    xaudio2.CriticalError += Xaudio2_CriticalError;

                    // DeviceDetails deviceDetails = xaudio2.GetDeviceDetails(0);

                    //xaudio2.CriticalError += Xaudio2_CriticalError;
                    if (!Disabled)
                    {
                        masteringVoice = new MasteringVoice(xaudio2, 0, 0);
                    }
                }
            }
            catch (Exception)
            {
                if (xaudio2 != null)
                {
                    xaudio2.Dispose();
                    xaudio2 = null;
                }
            }
            

            // based on the number of speakers reported, let's create the dictionary so that we can easily find out which speakers to play to when we play a sound

            if (xaudio2 != null && masteringVoice != null)
            {
                speakerChannels = masteringVoice.VoiceDetails.InputChannelCount;
                
                ProvisionSpeakers();
                
                masteringVoice.SetVolume(volume, 0);

                xaudio2.StartEngine();
            }
            else
            {
                Disabled = true;
            }
        }

        private static void Xaudio2_CriticalError(object sender, SharpDX.XAudio2.ErrorEventArgs e)
        {
            Disabled = true;
        }

        private static void ProvisionSpeakers()
        {
            switch (speakerChannels)
            {
                case 2:
                case 3:
                    // front left = 0
                    // front right = 1
                    // sub = 2?
                    speakerDictionary["left"] = new float[3] { 1.0f, 0.0f, 1.0f };
                    speakerDictionary["right"] = new float[3] { 0.0f, 1.0f, 1.0f };
                    // for the "center" channel, just make it be 7% of each speaker when it's in the center
                    speakerDictionary["center"] = new float[3] { 0.75f, 0.75f, 1.0f };
                    speakerDictionary["all"] = new float[3] { 1.0f, 1.0f, 1.0f };
                    break;
                case 6:
                    // front left/right same as 2.1
                    // center = 2
                    // subwoolfer 3, maybe?
                    // back left = 4 
                    // back right = 5
                    speakerDictionary["left"] = new float[6] { 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f };
                    speakerDictionary["right"] = new float[6] { 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f };
                    speakerDictionary["center"] = new float[6] { 0.0f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f };
                    speakerDictionary["all"] = new float[6] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
                    break;
                case 8:
                    // same as 5.1 only with side speakers
                    // side left = 6
                    // side right = 7
                    speakerDictionary["left"] = new float[8] { 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f };
                    speakerDictionary["right"] = new float[8] { 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f };
                    speakerDictionary["center"] = new float[8] { 0.0f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f };
                    speakerDictionary["all"] = new float[8] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
                    break;
                default:
                    speakerDictionary["left"] = new float[8] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
                    speakerDictionary["right"] = speakerDictionary["left"];
                    speakerDictionary["center"] = speakerDictionary["left"];
                    speakerDictionary["all"] = speakerDictionary["left"];
                    break;
            }

            /*
            case 4:
                    _panMatrix[0] = _panMatrix[2] = lVal;
                    _panMatrix[1] = _panMatrix[3] = rVal;
                    break;

                case 5:
                    _panMatrix[0] = _panMatrix[3] = lVal;
                    _panMatrix[1] = _panMatrix[4] = rVal;
                    break;
            */
        }

        public static void AddAudioFile(string objectPosition, string soundName, string soundFile, bool loop, bool resume, bool appPack, SharpDX.XAudio2.SourceVoice.VoidAction streamEndMethod)
        {
            if (!Disabled)
            {
                // if we don't have the sound loaded, let's grab it
                if (!LoadedSounds.ContainsKey(soundFile))
                {
                    // create a new sound
                    Sound sound = new Sound()
                    {
                        Resume = resume
                    };

                    // load the audio buffer for this sound
                    if (appPack)
                    {
                        AddBufferFromApplicationPack(sound, soundFile);
                    }
                    else
                    {
                        AddBufferFromPath(sound, soundFile);
                    }
                    
                    // if it loops, set it
                    if (loop)
                    {
                        sound.AudioBuffer.LoopCount = SharpDX.XAudio2.AudioBuffer.LoopInfinite;
                    }

                    //GetBuffer(soundfile);
                    SourceVoice sourceVoice = new SourceVoice(xaudio2, sound.AudioBuffer.WaveFormat, true);
                    sourceVoice.SetVolume(1.0f, SharpDX.XAudio2.XAudio2.CommitNow);

                    if (!string.IsNullOrWhiteSpace(objectPosition))
                    {
                        sourceVoice.SetOutputMatrix(sourceVoice.VoiceDetails.InputChannelCount, speakerChannels, speakerDictionary[objectPosition]);
                    }
                    
                    //sourceVoice.SubmitSourceBuffer(buffer, buffer.DecodedPacketsInfo);
                    //sourceVoice.Stop();
                    //sourceVoice.Start();

                    if (streamEndMethod != null)
                    {
                        sourceVoice.StreamEnd += streamEndMethod;
                    }
                    
                    sound.Voice = sourceVoice;

                    LoadedSounds.Add(soundName, sound);
                }
            }
        }

        public static void PauseSound(string soundKey)
        {
            if (!Disabled)
            {
                // get the sound
                Sound sound = LoadedSounds[soundKey];

                if (sound.Voice.State.BuffersQueued > 0)
                {
                    sound.Voice.Stop();

                    // if we're not going to resume it, just wipe it out here
                    if (!sound.Resume)
                    {
                        sound.Voice.FlushSourceBuffers();
                        sound.AudioBuffer.Stream.Position = 0;
                    }
                }
            }
        }

        public static bool StopSound(string soundKey)
        {
            if (!Disabled)
            {
                Sound s = LoadedSounds[soundKey];

                if (s.Voice.State.BuffersQueued > 0)
                {
                    s.Voice.Stop();
                    s.Voice.FlushSourceBuffers();
                    s.AudioBuffer.Stream.Position = 0;

                    return true;
                }
            }

            return false;
        }

        public static void ResumeSound(string soundKey)
        {
            if (!Disabled)
            {
                Sound sound = LoadedSounds[soundKey];

                if (sound.Voice.State.BuffersQueued > 0 && sound.Resume)
                {
                    sound.Voice.Start();
                }
            }
        }

        public static void ResetSound(string soundKey)
        {
            Sound sound = LoadedSounds[soundKey];

            sound.Voice.Stop();
            sound.Voice.FlushSourceBuffers();
        }

        public static void StopAllSounds()
        {
            if (!Disabled)
            {
                foreach (Sound sv in LoadedSounds.Values)
                {
                    sv.Voice.Stop();
                    sv.Voice.FlushSourceBuffers();
                    sv.AudioBuffer.Stream.Position = 0;
                }
            }
        }

        public static void PauseAllSounds()
        {
            if (!Disabled)
            {
                foreach (Sound sv in LoadedSounds.Values)
                {
                    sv.Voice.Stop();

                    // if it won't resume, let's just blow it away here
                    if (!sv.Resume)
                    {
                        sv.Voice.FlushSourceBuffers();
                        sv.AudioBuffer.Stream.Position = 0;
                    }
                }
            }
        }

        public static void ResumeAllSounds()
        {
            if (!Disabled)
            {
                foreach (Sound sv in LoadedSounds.Values)
                {
                    // don't bother with it unless it resumes
                    if (sv.Voice.State.BuffersQueued > 0 && sv.Resume)
                    {
                        sv.Voice.Start();
                    }
                }
            }
        }



        /*
        private static MasteringVoice m_MasteringVoice;
        public static MasteringVoice MasteringVoice
        {
            get
            {
                if (m_MasteringVoice == null)
                {
                    m_MasteringVoice = new MasteringVoice(XAudio);
                    m_MasteringVoice.SetVolume(1, 0);
                }
                return m_MasteringVoice;
            }
        }
        
        private static XAudio2 m_XAudio;

        public static XAudio2 XAudio
        {
            get
            {
                if (m_XAudio == null)
                {
                    m_XAudio = new XAudio2();
                    var voice = MasteringVoice; //touch voice to create it
                    m_XAudio.StartEngine();
                }
                return m_XAudio;
            }
        }
        */

        public static bool IsSoundBuffered(string soundKey)
        {
            if (!Disabled)
            {
                if (LoadedSounds[soundKey].Voice.State.BuffersQueued > 0)
                {
                    return true;
                }
            }
            
            return false;
        }

        public static void PlayOverLappingSound(string soundKey)
        {
            if (!Disabled)
            {
                AudioBufferAndMetaData buffer = LoadedSounds[soundKey].AudioBuffer;

                SourceVoice sv = new SourceVoice(xaudio2, buffer.WaveFormat, false);

                sv.SubmitSourceBuffer(buffer, buffer.DecodedPacketsInfo);
                //sv.Stop();
                sv.Start();
            }
        }

        
        public static void AdjustSpeakerAssignment(string soundFile, string objectPosition)
        {
            if (!Disabled)
            {
                SourceVoice sourceVoice = LoadedSounds[soundFile].Voice;

                if (sourceVoice.State.BuffersQueued > 0 && !string.IsNullOrWhiteSpace(objectPosition))
                {
                    sourceVoice.SetOutputMatrix(sourceVoice.VoiceDetails.InputChannelCount, speakerChannels, speakerDictionary[objectPosition]);
                }
            }
        }


        public static void PlaySound(string soundfile, string objectPosition)
        {
            if (!Disabled && LoadedSounds.ContainsKey(soundfile))
            {
                //await Task.Run(() =>
               // {
                    Sound sound = LoadedSounds[soundfile];

                    if (sound.Voice.State.BuffersQueued > 0)
                    {
                        sound.Voice.Stop();
                        sound.Voice.FlushSourceBuffers();
                        sound.AudioBuffer.Stream.Position = 0;
                    }

                    if (speakerChannels != 0 && !string.IsNullOrWhiteSpace(objectPosition))
                    {
                        sound.Voice.SetOutputMatrix(sound.Voice.VoiceDetails.InputChannelCount, speakerChannels, speakerDictionary[objectPosition]);
                    }

                    sound.Voice.SubmitSourceBuffer(LoadedSounds[soundfile].AudioBuffer, LoadedSounds[soundfile].AudioBuffer.DecodedPacketsInfo);

                    sound.Voice.Start();
               // });
            }
        }

        private static void SourceVoice_StreamEnd()
        {
            //System.Windows.MessageBox.Show("geeezus");
        }

        private static void AddBufferFromPath(Sound sound, string soundFilePath)
        {
            using (FileStream fsSource = new FileStream(soundFilePath, FileMode.Open, FileAccess.Read))
            {
                SoundStream soundstream = new SoundStream(fsSource);

                AudioBufferAndMetaData buffer = new AudioBufferAndMetaData()
                {
                    Stream = soundstream.ToDataStream(),
                    AudioBytes = (int)soundstream.Length,
                    Flags = BufferFlags.EndOfStream,
                    WaveFormat = soundstream.Format,
                    DecodedPacketsInfo = soundstream.DecodedPacketsInfo
                };

                sound.AudioBuffer = buffer;
            }
        }

        private static void AddBufferFromApplicationPack(Sound sound, string soundFile)
        {
            using (Stream audioStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Sounds/" + soundFile)).Stream)
            {
                SoundStream soundstream = new SoundStream(audioStream);

                AudioBufferAndMetaData buffer = new AudioBufferAndMetaData()
                {
                    Stream = soundstream.ToDataStream(),
                    AudioBytes = (int)soundstream.Length,
                    Flags = BufferFlags.EndOfStream,
                    WaveFormat = soundstream.Format,
                    DecodedPacketsInfo = soundstream.DecodedPacketsInfo
                };

                sound.AudioBuffer = buffer;
                //engine.AddSoundSourceFromIOStream(audioStream, soundName);   
            }
        }

        public static void Dispose()
        {
            if (masteringVoice != null && xaudio2 != null)
            {
                StopAllSounds();
                
                xaudio2.StopEngine();
                
                masteringVoice.Dispose();
                xaudio2.Dispose();
            }
        }
        
    }
}
