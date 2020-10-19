using System.Runtime.Serialization;

using BlockDefinitions.Types;

using UnityEngine;

namespace BlockDefinitions
{
	[DataContract]
	public class EngineBD : BlockDefinition
	{
		[DataMember]
		public readonly float FuelRequired = 0; // ???

		/// <summary>
		/// RedLine RPM
		/// </summary>
		[DataMember]
		public readonly float MaxRPM;

		[DataMember]
		public readonly float EnginePower;

		[DataMember]
		public readonly float TempCoefficient;

		[DataMember]
		public readonly float BaseFuelCost;

		/// <summary>
		/// Force exerted by the engine starter
		/// </summary>
		[DataMember]
		public readonly float StarterForce;

		#region Sound

		[DataMember]
		public readonly float SoundPitchRange;

		#region Sounds


		[DataMember]
		protected readonly string StarterSoundClipPath;

		[FetchDefinitionData(nameof(StarterSoundClipPath))]
		public AudioClip StarterSoundClip { get; private set; }


		[DataMember]
		protected readonly string IdleSoundClipPath;

		[FetchDefinitionData(nameof(IdleSoundClipPath))]
		public AudioClip IdleSoundClip { get; private set; }


		[DataMember]
		protected readonly string RunningSoundClipPath;

		[FetchDefinitionData(nameof(RunningSoundClipPath))]
		public AudioClip RunningSoundClip { get; private set; }

		#endregion Sounds
		#endregion Sound


		/// <inheritdoc />
		public EngineBD(BlockID blockID, float mass, string name, string description, string meshFilePath) : base
			(blockID, mass, name, description, meshFilePath)
		{
			StarterSoundClipPath = "Audio/engineV6_starter.ogg";
			IdleSoundClipPath = "Audio/engineV6_idle_loop.ogg";
			RunningSoundClipPath = "Audio/engineV6RunningLoop.ogg"; // Ewww naming inconsistency!
		}
	}
}
