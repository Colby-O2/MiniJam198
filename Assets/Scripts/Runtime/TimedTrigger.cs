using UnityEngine;

namespace MJ198
{
    public class TimedTrigger
    {
        private bool _isPaused = false;
        private float _startTime = 0;
        private float _pauseStartTime = 0;
        private float _pauseDuration = 0;

        private float GetRawTime()
        {
            return Time.time - _startTime;
        }
        
        public float Pause()
        {
            if (_isPaused) return -1;
            _isPaused = true;
            _pauseStartTime = GetRawTime();
            return Now();
        }

        public float Unpause()
        {
            if (!_isPaused) return -1;
            float cTime = GetRawTime();
            _isPaused = false;
            _pauseDuration += cTime - _pauseStartTime;
            return Now();
        }

        public float Now()
        {
            if (_isPaused) {
                return _pauseStartTime - _pauseDuration;
            } else {
                return GetRawTime() - _pauseDuration;
            }
        }

        private float Reset()
        {
			float res = Now();
            _startTime = Time.time;
            _isPaused = true;
            _pauseStartTime = 0;
            _pauseDuration = 0;
			return res;
        }

        public bool Try(float length)
        {
            if (Now() < length) return false;
            Reset();
            Unpause();
            return true;
        }

        public bool IsPaused() => _isPaused;
    }
}
