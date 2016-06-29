﻿using System;

namespace HAARP.Samplers
{
	internal unsafe class VocalSampler : Sampler
	{
		private float sampleIn, sampleOut;

		private readonly VoiceData _voice;
		private readonly BandPassFilter[] bands;
		private readonly ButterworthFilter lowPass, highPass;
		private readonly int numBands;

		private const float LowResonance = .2f;
		private const float HighResonance = 0.09f;

		private const float LEVEL_F1 = .01850f;
		private const float LEVEL_F2 = .01125f;
		private const float LEVEL_F3 = .01250f;
		private const float LEVEL_F4 = .00750f;

		public VocalSampler(Synthesizer synth, long seed) : base(synth)
		{
			bands = new[]
			{
				// open-mid back unrounded vowel
				//new BandPassFilter(2600, 2700, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F3},
				//new BandPassFilter(1170, 1170, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F2},
				//new BandPassFilter(600, 600, synth.SampleRate, LowResonance, LowResonance) {Volume = LEVEL_F1},

				// open-mid front unrounded vowel
				new BandPassFilter(3500, 3600, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F4}, 
				new BandPassFilter(2600, 2700, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F3},
				new BandPassFilter(1800, 1900, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F2},
				new BandPassFilter(650, 800, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F1},

				// close front rounded vowel
				//new BandPassFilter(3200, 3300, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F4},
				//new BandPassFilter(2100, 2200, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F3},
				//new BandPassFilter(1800, 1900, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F2},
				//new BandPassFilter(200, 300, synth.SampleRate, LowResonance, LowResonance) {Volume = LEVEL_F1},
				
				// near-open front unrounded vowel
				//new BandPassFilter(2900, 3000, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F3},
				//new BandPassFilter(1700, 1800, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F2},
				//new BandPassFilter(800, 900, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F1},
				 
				// postalveolar approximant
				//new BandPassFilter(3000, 3000, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F3},
				//new BandPassFilter(1400, 1400, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F2},
				//new BandPassFilter(350, 350, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F1},

				// close-mid front unrounded vowel
				//new BandPassFilter(3600, 3700, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F4},
				//new BandPassFilter(2500, 2600, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F3},
				//new BandPassFilter(2200, 2300, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F2},
				//new BandPassFilter(450, 500, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F1},

				// close front unrounded vowel
				//new BandPassFilter(3000, 3000, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F3},
				//new BandPassFilter(2250, 2250, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F2},
				//new BandPassFilter(250, 250, synth.SampleRate, HighResonance, HighResonance) {Volume = LEVEL_F1},

				// voiceless palatal-alveolar sibilant fricative
                //new BandPassFilter(200, 1200, synth.SampleRate, 1.0f, 1.0f) { Volume = 0.075f }, 
                //new BandPassFilter(1900, 5500, synth.SampleRate, .35f, .35f) { Volume = 0.01f },
				//new BandPassFilter(2100, 2500, synth.SampleRate, .2f, .2f) { Volume = 0.1f },
				//new BandPassFilter(3700, 4100, synth.SampleRate, .2f, .2f) { Volume = 0.050f },
				//new BandPassFilter(4600, 5400, synth.SampleRate, .18f, .18f) { Volume = 0.060f },
				//new BandPassFilter(6700, 7200, synth.SampleRate, .1f, .1f) { Volume = 0.0060f }, 
            };

			lowPass = new ButterworthFilter(500, synth.SampleRate, PassFilterType.LowPass, .1f);
			highPass = new ButterworthFilter(6000, synth.SampleRate, PassFilterType.HighPass, .4f);

			numBands = bands.Length;
			_voice = VoiceData.Get(Voice.Jimmy);
		}

		public override void Update(ref float sample)
		{
			synth.Fundamental -= 20.0f * synth.TimeStep;
			sampleOut = 0f;

			// ha ha ha ha ha ha
			float m = ((float)Math.Sin(synth.TimePosition * Math.PI * 8.0f) + 1.0f) / 2.0f;
			sampleIn = sample * m
				+ NoiseSampler.NoiseDataPointer[synth.Position % NoiseSampler.NoiseDataLength] * synth.Voice.FricativeForce * (0.2f + 0.55f * (float)Math.Pow(1.0f - m, 2));

			// Update filters
			for (int i = 0; i < numBands; i++)
			{
				bands[i].Update(sampleIn);
				sampleOut += bands[i].Value;
			}

			lowPass.Update(sampleIn);
			highPass.Update(sampleIn);
			sampleOut += lowPass.Value * .075f;
			sampleOut += highPass.Value * .015f;

			sample = sampleOut;
		}
	}
}