﻿using System;
using System.Collections.Generic;
namespace Framework
{
    public class NetworkManager : FrameworkModule<NetworkManager>
    {
        private List<INetworkChannel> mChannels = new List<INetworkChannel>();
        
        protected override void OnDestroy()
        {
            for (int i = 0; i < mChannels.Count; ++i)
            {
                mChannels[i].Disconnect();
            }
            mChannels.Clear();
        }

        protected override void Update(float deltaTime, float unscaledDeltaTime)
        {
            for(int i = 0;i< mChannels.Count;++i)
            {
                mChannels[i].Update(unscaledDeltaTime);
            }
        }
        

        public void AddNetworkChannel(INetworkChannel channel)
        {
            for (int i = 0; i < mChannels.Count;++i)
            {
                if(mChannels[i].Name == channel.Name)
                {
                    throw new Exception(string.Format("exist NetworkChannel name {0} type is {1}!", channel.Name, mChannels[i].GetType()));
                }
            }
            mChannels.Add(channel);
        }
        
    }
}
