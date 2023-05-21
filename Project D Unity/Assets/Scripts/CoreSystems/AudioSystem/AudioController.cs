using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace CoreSystems.AudioSystem
{
    /// <summary>
    /// Class that controls the execution of more complex audio jobs: delay, fade...
    /// </summary>
    /// <remarks>Singleton</remarks>
    //TODO: redefine the code so that I can have AudioSources in different objects in scene
    public class AudioController : MonoBehaviour
    {
        //Members 
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static AudioController instance;
        
        /// <summary>
        /// Debug mode to show or hide messages in Unity console
        /// </summary>
        public bool debug;
        //public AudioTrack[] tracks;
        
        /// <summary>
        /// Hashtable containing a relation between audio types and gameobjects and track
        /// </summary>
        /// <seealso cref="AudioType"/>
        /// <seealso cref="AudioTrack"/>
        private Hashtable _audioTable;  //relationship between audio types and GameObjects (key) and tracks (value)
        
        /// <summary>
        /// Hashtable containing a relation between audio types and gameobjects and audio jobs
        /// </summary>
        /// <seealso cref="AudioType"/>
        /// <seealso cref="AudioJob"/>
        private Hashtable _jobTable;    //relationship between audio types and GameObjects (key) and jobs (value) (coroutines)
        
        
    #region Unity functions

        private void Awake()
        {
            if (!instance)
            {
                Configure();
            }
            else
            {
                Destroy(gameObject); //No need to have a duplicated singleton
            }
        }

        private void OnDisable()
        {
            Dispose();
        }

    #endregion
    
    #region Public functions

        //TODO: quite a bit of repeated code. Check what can be done to solve it
        //The way this could be done could be by creating a wrapping method where all the parameters are specified
        //plus the addition of a new parameter that would be the action to be done and then create the specific job.
        //The problem about this is that the objects in the scene would have to know about the jobs the AudioController
        //works with and it does not seem like a good solution
        /// <summary>
        /// Plays the audio given by the audio type
        /// </summary>
        /// <param name="type">The type of the audio</param>
        /// <param name="requestingObject">The object requesting the job</param>
        /// <param name="fade">The fade duration</param>
        /// <param name="delay">The delay before starting the job</param>
        public void PlayAudio(AudioType type, GameObject requestingObject , float fade = 0, float delay = 0)
        {
            AddJob(new AudioJob(AudioAction.START, type, requestingObject ,fade, delay));
        }

        /// <summary>
        /// Stops the audio given by the audio type
        /// </summary>
        /// <param name="type">The type of the audio</param>
        /// <param name="requestingObject">The object requesting the job</param>
        /// <param name="fade">The fade duration</param>
        /// <param name="delay">The delay before starting the job</param> 
        public void StopAudio(AudioType type, GameObject requestingObject, float fade = 0, float delay = 0)
        {
            AddJob(new AudioJob(AudioAction.STOP, type, requestingObject , fade, delay )); 
        }

        /// <summary>
        /// Restarts the audio given by the audio type
        /// </summary>
        /// <param name="type">The type of the audio</param>
        /// <param name="requestingObject">The object requesting the job</param>
        /// <param name="fade">The fade duration</param>
        /// <param name="delay">The delay before starting the job</param> 
        public void RestartAudio(AudioType type, GameObject requestingObject , float fade = 0,  float delay = 0 )
        {
            AddJob(new AudioJob(AudioAction.RESTART, type, requestingObject , fade, delay )); 
        }

        /// <summary>
        /// Adds a series of tracks specified by a GameObject to the respective hashtables
        /// </summary>
        /// <param name="tracks">The tracks to be added</param>
        /// <param name="addingObject">The object adding the tracks</param>
        public void AddTracks(AudioTrack[] tracks, GameObject addingObject)
        {
            foreach (var track in tracks)
            {
                foreach (var audioObject in track.audios)
                {
                    if (audioObject.type == AudioType.NONE)
                    {
                        LogWarning("You are trying to register an audio with type: ["+AudioType.NONE+"] by" + addingObject.name);
                        return;
                    }

                    if (audioObject.clip == null)
                    {
                        LogWarning("You are trying to add a track with no clip attached: ["+audioObject.type+"] by: " + addingObject.name);
                        return;
                    }
                    
                    //do not want duplicated keys
                    if (_audioTable.Contains(audioObject.type))
                    {
                        LogWarning("You are trying to register an audio that already exist. Type: ["+audioObject.type +"]");
                        continue;
                    }

                    Tuple<AudioType, GameObject> key = new Tuple<AudioType,GameObject>(audioObject.type, addingObject);
                    _audioTable.Add(key, track);
                    Log("Registering audio of type: ["+audioObject.type+"]");
                }
            }     
        }
        
    #endregion

    #region Private functions

        /// <summary>
        /// Configures an initial instance of the class
        /// </summary>
        private void Configure()
        {
            instance = this;

            _audioTable = new Hashtable();
            _jobTable = new Hashtable();

            //AddTracks(tracks, gameObject);
        }

        /// <summary>
        /// Stops every job that is still running
        /// </summary>
        private void Dispose()
        {
            if (_jobTable == null)
                return;
            
            foreach (DictionaryEntry entry in _jobTable)
            {
                IEnumerator job = (IEnumerator) entry.Value;
                StopCoroutine(job);
            }
        }

        /// <summary>
        /// Adds a new job to the jobs' hashtable by removing the conflicting jobs and starting the new one
        /// </summary>
        /// <param name="job">The job to be added</param>
        /// <seealso cref="RemoveConflictingJobs"/>
        /// <seealso cref="RunAudioJob"/>
        private void AddJob(AudioJob job)
        {
            if (job.type == AudioType.NONE)
            {
                LogWarning("You are trying to add a job of type [NONE]");
                return;
            }

            var jobKey = new Tuple<AudioType, GameObject>(job.type, job.requester);

            if (!_audioTable.Contains(jobKey))
            {
                LogWarning("The audio table does not contain a valid entry for the job from: "+job.requester.name+".");
                return;
            }
            //Remove conflicting jobs
            RemoveConflictingJobs(jobKey);
            //Start job
            IEnumerator jobRunner = RunAudioJob(job);
            _jobTable.Add(jobKey, jobRunner);
            StartCoroutine(jobRunner);
            Log("Job added to the jobTable with type: ["+job.type+"] and operation: ["+job.action+"]");
        }
        
        /// <summary>
        /// Removes a job from the job table
        /// </summary>
        /// <param name="jobKey">A tuple referencing the audio type and and the game object requesting</param>
        private void RemoveJob(Tuple<AudioType, GameObject> jobKey)
        {
            if (!_jobTable.Contains(jobKey))
            {
                LogWarning("You are trying to remove a job ["+jobKey.Item1+"] that is not running");
                return;
            }

            IEnumerator runningJob = (IEnumerator) _jobTable[jobKey];
            StopCoroutine(runningJob);
            _jobTable.Remove(jobKey); //Remove it from the job table too
            Log("Job of type ["+jobKey.Item1+"] removed");
        } 
        
        /// <summary>
        /// Removes the conflicting jobs either because there is an audio of the same type running or
        /// the source this type uses in being used by a different audio type.
        /// </summary>
        /// <param name="jobKey">A tuple referencing the audio type and and the game object requesting </param>
        private void RemoveConflictingJobs(Tuple<AudioType, GameObject> jobKey)
        {
            if (_jobTable.Contains(jobKey)) //The conflicting job can be an audio of the same type
            {
                RemoveJob(jobKey);
            }

            //In other case we need to iterate through the different audios to check if the audio source of the
            //audio we want to be played is being used
            var conflictAudio = new Tuple<AudioType, GameObject>(AudioType.NONE, jobKey.Item2);
            foreach (DictionaryEntry job in _jobTable)
            {
                var audioKey = (Tuple<AudioType, GameObject>) job.Key;
                var audioTrackInUse = (AudioTrack)_audioTable[audioKey];
                var audioTrackNeeded = (AudioTrack) _audioTable[jobKey];

                if (audioTrackNeeded.source == audioTrackInUse.source)
                {
                    conflictAudio = new Tuple<AudioType, GameObject>(audioKey.Item1, jobKey.Item2);
                    break; //We have found it. No need to keep iterating
                }
            }

            if (conflictAudio.Item1 !=  AudioType.NONE)
            {
                RemoveJob(conflictAudio);
                Log("Job of type ["+conflictAudio+"] removed"); 
            }
        }

        /// <summary>
        /// It runs the logic behind the specifications of a job
        /// </summary>
        /// <param name="job">The job's specification to be played</param>
        /// <returns>The job that can be run</returns>
        /// <remarks>It does not start the coroutine</remarks>
        //TODO: The method seems way to extensive. Modularize it
        private IEnumerator RunAudioJob(AudioJob job)
        {
            //Play the delay that has been set to the job
            yield return new WaitForSeconds(job.delay);

            var key = new Tuple<AudioType, GameObject>(job.type, job.requester);
            AudioTrack track = (AudioTrack) _audioTable[key];
            track.source.clip = GetAudioClipFromAudioTrack(job.type, track);

            switch (job.action)
            {
                case AudioAction.START:
                    track.source.Play();
                    break;
                
                case AudioAction.STOP:
                    //The audio can not be stopped immediately. There would be no fading in that case
                    if(job.fade == 0) track.source.Stop();
                    break;
                
                case AudioAction.RESTART:
                    track.source.Stop();
                    track.source.Play();
                    break;
            }

            if (job.fade > 0)
            {
                float initialVolume = job.action is AudioAction.START or AudioAction.RESTART ? 0 : track.source.volume;
                float targetVolume = initialVolume == 0 ? track.source.volume : 0;
                
                float timeElapsed = 0f;

                while (timeElapsed <= job.fade)
                {
                    track.source.volume = Mathf.Lerp(initialVolume, targetVolume, timeElapsed / job.fade);
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
                
                if (job.action == AudioAction.STOP)
                {
                    track.source.Stop();
                    track.source.volume = initialVolume; //In case I want to restart the sound with no fading
                }
            }
            
            _jobTable.Remove(new Tuple<AudioType, GameObject>(job.type, job.requester));
            Log("Job count: "+_jobTable.Count);

            //yield return null;
        }

        /// <summary>
        /// Gets an audio clip from an AudioTrack
        /// </summary>
        /// <param name="type">The type of the clip that is wanted</param>
        /// <param name="track">The track in which to look for</param>
        /// <returns>The audio clip if it is found or null if not</returns>
        private AudioClip GetAudioClipFromAudioTrack(AudioType type, AudioTrack track)
        {
            foreach (var audioObject in track.audios)
            {
                if (audioObject.type == type)
                {
                    return audioObject.clip;
                }
            }

            LogWarning("There is no AudioClip with type: [" + type + "]");
            return null;
        }

        /// <summary>
        /// Logs printing method
        /// </summary>
        /// <param name="msg">The message to print</param>
        private void Log(string msg)
        {
            if (!debug) return;
            Debug.Log("[AudioController]: " + msg);
        }

        /// <summary>
        /// Warning logs printing method
        /// </summary>
        /// <param name="msg"></param>
        private void LogWarning(string msg)
        {
            if (!debug) return;
            Debug.LogWarning("[AudioController]: " + msg); 
        }
        
    #endregion

        /// <summary>
        /// It specifies the information a job must handle: fading, delay...
        /// </summary>
        /// <seealso cref="AudioController"/>
        private class AudioJob
        {
            public readonly AudioAction action;
            public readonly AudioType type;
            public readonly GameObject requester;
            public readonly float fade;
            public readonly float delay; 
            
            public AudioJob(AudioAction action, AudioType type, GameObject requester, float fade, float delay)
            {
                this.action = action;
                this.type = type;
                this.requester = requester;

                if (fade < 0)
                {
                    fade = 0;
                    Debug.LogWarning("[AudioJob]: You cannot set a fade value smaller than 0");
                }
                this.fade = fade;
                this.delay = delay;
            }

        }

        /// <summary>
        /// All the different actions a job can do
        /// </summary>
        private enum AudioAction
        {
            START,
            STOP,
            RESTART,
        }
    }
} 