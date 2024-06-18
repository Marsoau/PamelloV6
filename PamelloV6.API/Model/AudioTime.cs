﻿namespace PamelloV6.API.Model
{
	public class AudioTime
	{
		public static long FrequencyMultiplier = 96000;

		public long TimeValue;

		public int TotalSeconds {
			get => (int)(TimeValue / FrequencyMultiplier);
			set => TimeValue = value * FrequencyMultiplier;
		}
		public int Seconds {
			get => TotalSeconds % 60;
			set => TotalSeconds = TotalSeconds - Seconds + value;
		}

		public int TotalMinutes {
			get => TotalSeconds / 60;
			set => TimeValue = value * 60 * FrequencyMultiplier;
		}
		public int Minutes {
			get => TotalMinutes % 60;
			set => TotalMinutes = TotalMinutes - Minutes + value;
		}

		public int TotalHours {
			get => TotalMinutes / 60;
			set => TimeValue = value * 3600 * FrequencyMultiplier;
		}

		public AudioTime(long timeValue) {
			TimeValue = timeValue;
		}
		public AudioTime(int seconds) {
			TotalSeconds = seconds;
		}
		public AudioTime(int minutes, int seconds) {
			TotalSeconds = seconds + minutes * 60;
		}
		public AudioTime(int hours, int minutes, int seconds) {
			TotalSeconds = seconds + minutes * 60 + hours * 3600;
		}


		public override string ToString() {
			return $"{((TotalHours < 10) ? '0' : "")}{TotalHours}:{((Minutes < 10) ? '0' : "")}{Minutes}:{((Seconds < 10) ? '0' : "")}{Seconds}";
		}
		public string ToShortString() {
			if (TotalHours != 0)
				return $"{((TotalHours < 10) ? '0' : "")}{TotalHours}:{((Minutes < 10) ? '0' : "")}{Minutes}:{((Seconds < 10) ? '0' : "")}{Seconds}";
			else
				return $"{((Minutes < 10) ? '0' : "")}{Minutes}:{((Seconds < 10) ? '0' : "")}{Seconds}";
		}
	}
}