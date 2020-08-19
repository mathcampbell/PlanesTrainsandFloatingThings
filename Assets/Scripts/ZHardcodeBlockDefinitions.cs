using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BlockDefinitions;


	/// <summary>
	/// This class contains C# hardcoded <see cref="BlockDefinition"/>s, for testing purposes.
	/// </summary>
	public static class ZHardcodeBlockDefinitions
	{
		public static List<BlockDefinition> MainDefinitions()
		{
			var list = new List<BlockDefinition>
			{
				new BlockDefinition(1,   10, "TestBlock",  "A block for testing.")
				, new BlockDefinition(2, 10, "TestBlock2", "A 2nd block for testing.")
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

