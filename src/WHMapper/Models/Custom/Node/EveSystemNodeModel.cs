﻿using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Xml.Linq;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using WHMapper.Models.DTO.EveMapper.Enums;
using WHMapper.Models.Db;
using WHMapper.Models.DTO.EveAPI.Universe;
using WHMapper.Models.DTO.EveMapper;

namespace WHMapper.Models.Custom.Node
{
    public class EveSystemNodeModel : NodeModel
    {
        public event Action<EveSystemNodeModel>? OnLocked;

        private WHSystem _wh = null!;

        public int IdWH
        {
            get
            {
                return _wh.Id;

            }
        }

        public int IdWHMap
        {
            get
            {
                return _wh.WHMapId;

            }
        }

        public String Name
        {
            get
            {
                return _wh.Name;
            }
         }

        public String? NameExtension
        {
            get
            {
                if(_wh!=null && _wh.NameExtension!=0)
                    return Convert.ToChar(_wh.NameExtension).ToString();
                return null;
            }
        }

        
        public int SolarSystemId
        {
            get
            {
                return _wh.SoloarSystemId;
            }
        }

        public float SecurityStatus
        {
            get
            {
                if (_wh.SecurityStatus <= -0.99)
                    return -1;
                else
                    return (float)Math.Round(_wh.SecurityStatus, 1);

            }
        }

        
        public new bool Locked
        {
            get
            {
                return base.Locked;
            }
            set
            {
                if (base.Locked != value)
                {
                    base.Locked = value;
                    OnLocked?.Invoke(this);
                }
            }

        }

        public string RegionName { get; private set; }
        public string ConstellationName { get; private set;}

        public EveSystemType SystemType { get; private set; } = EveSystemType.None;
        public WHEffect Effect { get; private set; } = WHEffect.None;
        public IList<EveSystemEffect>? EffectDetails { get; private set; } = null!;
        public IList<WHStatic>? Statics { get; private set; } = null!;
        public BlockingCollection<string> ConnectedUsers { get; private set; } = new BlockingCollection<string>();


        public EveSystemNodeModel(WHSystem wh,string regionName, string constellationName, EveSystemType systemType, WHEffect whEffect, IList<EveSystemEffect>? effectDetails, IList<WHStatic>? whStatics) 
        {
            _wh = wh;
            RegionName = regionName;
            ConstellationName = constellationName;

            Title = this.Name;
            SystemType = systemType;
            Effect = whEffect;
            EffectDetails = effectDetails;
            Statics = whStatics;
            Locked = wh.Locked;

            AddPort(PortAlignment.Bottom);
            AddPort(PortAlignment.Top);
            AddPort(PortAlignment.Left);
            AddPort(PortAlignment.Right);
        }

        public EveSystemNodeModel(WHSystem wh, string regionName, string constellationName)
        {
            _wh = wh;
            RegionName = regionName;
            ConstellationName = constellationName;

            Title = this.Name;
            Locked = wh.Locked;

            
            if (SecurityStatus >= 0.5)
                SystemType = EveSystemType.HS;
            else if (SecurityStatus < 0.5 && SecurityStatus > 0)
                SystemType = EveSystemType.LS;
            else
                SystemType = EveSystemType.NS;
            

            AddPort(PortAlignment.Bottom);
            AddPort(PortAlignment.Top);
            AddPort(PortAlignment.Left);
            AddPort(PortAlignment.Right);
        }


        public void IncrementNameExtension()
        {
            if (_wh.NameExtension == 0)
                _wh.NameExtension = Convert.ToByte('A');
            else
            {
                if (_wh.NameExtension != Convert.ToByte('Z'))
                    _wh.NameExtension++;
                else
                    _wh.NameExtension = Convert.ToByte('Z');
            }
        }

        public void DecrementNameExtension()
        {
            if (_wh.NameExtension > Convert.ToByte('A'))
                _wh.NameExtension--;
            else
                _wh.NameExtension = 0;
        }


        public async Task AddConnectedUser(string userName)
        {
            if (!ConnectedUsers.Contains(userName))
            {
                while (!ConnectedUsers.TryAdd(userName))
                    await Task.Delay(1);

                this.Refresh();
            }
        }

        public async Task RemoveConnectedUser(string userName)
        {
            if (ConnectedUsers.Contains(userName))
            {

                string comparedItem;
                var itemsList = new List<string>();
                do
                {

                    while (!ConnectedUsers.TryTake(out comparedItem))
                        await Task.Delay(1);

                    if (!comparedItem.Equals(userName))
                    {
                        itemsList.Add(comparedItem);
                    }
                } while (!(comparedItem.Equals(userName)));
                Parallel.ForEach(itemsList, async t => await AddConnectedUser(t));

                this.Refresh();
            }
        }
    }
}

