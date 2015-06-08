﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Channel.World;
using Aura.Channel.World.Dungeons;
using Aura.Channel.World.Entities;
using Aura.Shared.Scripting.Scripts;
using Aura.Shared.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Channel.Scripting.Scripts
{
	public class DungeonScript : IScript
	{
		/// <summary>
		/// Name of the dungeon
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Called when the script is initially created.
		/// </summary>
		/// <returns></returns>
		public bool Init()
		{
			var attr = this.GetType().GetCustomAttribute<DungeonScriptAttribute>();
			if (attr == null)
			{
				Log.Error("DungeonScript.Init: Missing DungeonScript attribute.");
				return false;
			}

			this.Name = attr.Name;

			this.Load();

			ChannelServer.Instance.ScriptManager.DungeonScripts.Add(this.Name, this);

			return true;
		}

		/// <summary>
		/// Called from Init.
		/// </summary>
		public virtual void Load()
		{
		}

		public virtual bool Route(Creature creature, Item item, ref string dungeonName)
		{
			return true;
		}

		/// <summary>
		/// Called when the dungeon was just created.
		/// </summary>
		public virtual void OnCreation(Dungeon dungeon)
		{
		}

		/// <summary>
		/// Called when the boss door opens.
		/// </summary>
		public virtual void OnBoss(Dungeon dungeon)
		{
		}

		/// <summary>
		/// Called when one of the boss monsters dies.
		/// </summary>
		public virtual void OnBossDeath(Dungeon dungeon, Creature deadBoss)
		{
		}

		/// <summary>
		/// Called when the boss was killed.
		/// </summary>
		public virtual void OnCleared(Dungeon dungeon)
		{
		}

		/// <summary>
		/// Called when a player leaves a dungeon via the first statue,
		/// logging out, or similar.
		/// </summary>
		public virtual void OnLeftEarly(Dungeon dungeon, Creature creature)
		{
		}
	}

	public class DungeonScriptAttribute : Attribute
	{
		public string Name { get; private set; }

		public DungeonScriptAttribute(string name)
		{
			this.Name = name.ToLower();
		}
	}
}
