﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace NetworkSocket.WebSocket
{
    /// <summary>
    /// WebSocket会话对象
    /// </summary>
    public class WebSocketSession : SessionBase
    {
        /// <summary>
        /// 发送回复数据
        /// </summary>
        /// <exception cref="SocketException"></exception>
        /// <param name="response">回复内容</param>
        public void SendResponse(Response response)
        {
            this.Send(response.ToBytes());
        }


        /// <summary>
        /// 发送文本消息
        /// </summary>     
        /// <param name="content">文本内容</param>
        /// <exception cref="SocketException"></exception>
        public void SendText(string content)
        {
            var bytes = content == null ? new byte[0] : Encoding.UTF8.GetBytes(content);
            var response = new FrameResponse(FrameCodes.Text, bytes);
            this.SendResponse(response);
        }

        /// <summary>
        /// 发送二进制数据
        /// </summary>       
        /// <param name="content">二进制数据</param>
        /// <exception cref="SocketException"></exception>
        public void SendBinary(byte[] content)
        {
            var response = new FrameResponse(FrameCodes.Binary, content);
            this.SendResponse(response);
        }



        /// <summary>
        /// 尝试发送回复数据
        /// </summary>        
        /// <param name="response">回复内容</param>
        public bool TrySendResponse(Response response)
        {
            return this.TrySend(response.ToBytes());
        }

        /// <summary>
        /// 尝试发送文本消息
        /// </summary>       
        /// <param name="content">文本内容</param>
        public bool TrySendText(string content)
        {
            try
            {
                this.SendText(content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试发送二进制数据
        /// </summary>       
        /// <param name="content">二进制数据</param>     
        public bool TrySendBinary(byte[] content)
        {
            try
            {
                this.SendBinary(content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 正常关闭客户端
        /// </summary>       
        /// <param name="code">关闭码</param>
        public void Close(StatusCodes code)
        {
            this.Close(code, string.Empty);
        }

        /// <summary>
        /// 正常关闭客户端
        /// </summary>      
        /// <param name="code">关闭码</param>
        /// <param name="reason">原因</param>
        public void Close(StatusCodes code, string reason)
        {
            var codeBytes = ByteConverter.ToBytes((ushort)(code), Endians.Big);
            var reasonBytes = Encoding.UTF8.GetBytes(reason ?? string.Empty);
            var content = new byte[codeBytes.Length + reasonBytes.Length];

            Array.Copy(codeBytes, 0, content, 0, codeBytes.Length);
            Array.Copy(reasonBytes, 0, content, codeBytes.Length, reasonBytes.Length);

            var response = new FrameResponse(FrameCodes.Close, content);
            this.TrySendResponse(response);
            this.Close();
        }
    }
}