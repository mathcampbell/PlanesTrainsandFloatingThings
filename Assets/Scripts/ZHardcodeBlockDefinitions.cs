﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BlockDefinitions;

using Tools;


	/// <summary>
	/// This class contains C# hardcoded <see cref="BlockDefinition"/>s, for testing purposes.
	/// </summary>
	public static class ZHardcodeBlockDefinitions
	{
		public static List<BlockDefinition> MainDefinitions()
		{
			BlockID id = 1;
			var list = new List<BlockDefinition>
			{
				new BlockDefinition(id++,   10, "Cube",  "A Cube", "Models/Block_Primitives/Block.obj", "Materials/Block Materials/Block.mat")
				, new BlockDefinition(id++, 10, "Wedge", "Wedge", "Models/Block_Primitives/1x1wedge.obj", "Materials/Block Materials/Block.mat")
				, new BlockDefinition(id++, 10, "Triangle corner", "Triangle corner", "Models/Block_Primitives/1x1pyramid.obj", "Materials/Block Materials/Block.mat")
				, new EngineBD(id++, 100, "Engine V6", "Engine with sounds and a model.", "Models/Block_Primitives/EngineV6.fbx")
			};

			{ // Sanity check
				var usedIds = new Dictionary<BlockID, BlockDefinition>();
				foreach (var definition in list)
				{
					if (false == usedIds.TryAdd(definition.BlockID, definition))
					{
						throw new InvalidOperationException($"Definition {definition.Name} uses already assigned BlockID {(uint)definition.BlockID}");
					}
				}
			}
			return list;
		}
	}
