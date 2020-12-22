using BlockDefinitions;

using UnityEngine;

using Vehicle;
using Vehicle.Blocks;

namespace Testing {
	/// <summary>
	/// This class contains C# hardcoded <see cref="VehicleData"/>s, for testing purposes.
	/// </summary>
	public static class ZHardcodeVehicleDesigns
	{

		public static VehicleData vehicle1;




		static ZHardcodeVehicleDesigns()
		{
			Vehicle1();
		}

		private static BlockDefinition cubeDefinition = BlockDefinition.Definitions[1];



		private static Block CubeAt(Vector3Int position)
		{
			Block b = new Block(cubeDefinition);
			b.position = position;
			return b;
		}

		private static void Vehicle1()
		{
			vehicle1 = new VehicleData();
			var d = vehicle1;


			int i = -5;
			void AddCube()
			{
				var b = CubeAt(new Vector3Int(i++, 0, 0));
				d.blocks.Add(b);
			}

			AddCube(); AddCube(); AddCube(); AddCube(); AddCube(); AddCube(); AddCube(); AddCube(); AddCube(); AddCube();
		}
	}
}

