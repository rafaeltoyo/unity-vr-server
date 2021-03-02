﻿using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using HandTracking.Models;

namespace HandTracking.Parser
{
    public class MediapipeDataParser : IHandTrackingDataParser
    {
        private readonly Regex JOINT_MASK = new Regex(@"^[a-zA-Z_]+(;[0-9,.\-+e]+){3}");

        private List<HandJoint> _joints = new List<HandJoint>();

        /// <summary>
        ///     Método responsável por converter a String de dados, recebida do serviço de handtracking, para objeto Hand.
        /// </summary>
        /// <param name="coordenates">String recebida com os dados</param>
        /// <returns>Objeto Hand</returns>
        public Hand Parse(string coordenates)
        {
            return ParseHand(coordenates);
        }

        /// <summary>
        ///     Parse a hand from coordenates string
        /// </summary>
        /// <param name="coordenates">String with joints</param>
        /// <returns>Parsed Hand</returns>
        private Hand ParseHand(string coordenates)
        {
            List<HandJoint> joints = new List<HandJoint>();
            
            foreach (string joint in coordenates.Split('|'))
            {
                HandJoint hj = ParseJoint(joint);
                if (hj != null)
                    joints.Add(hj);
            }
            if (joints.Count > 0) {
                return new Hand(
                    joints[(ushort) MediapipeJoints.WRIST],
                    ParseFinger(MediapipeFingers.THUMB, joints),
                    ParseFinger(MediapipeFingers.INDEX, joints),
                    ParseFinger(MediapipeFingers.MIDDLE, joints),
                    ParseFinger(MediapipeFingers.RING, joints),
                    ParseFinger(MediapipeFingers.PINKY, joints)
                );
            } else {
                return null;
            }
        }

        /// <summary>
        ///     Parse a joint from coordenate string
        /// </summary>
        /// <param name="coordenates">String with name, x, y and z</param>
        /// <returns>Parsed joint</returns>
        private HandJoint ParseJoint(string coordenates)
        {
            if (JOINT_MASK.IsMatch(coordenates))
            {
                string[] values = coordenates.Split(';');
                return new HandJoint(values[0])
                    .Update(
                        ParseFloat(values[1]) * 3,
                        ParseFloat(values[2]) * 3 * -1,
                        ParseFloat(values[3]) * 3
                    );
            }
            return null;
        }

        /// <summary>
        ///     Parse a finger from its name and hand's joints
        /// </summary>
        /// <param name="finger">Finger to parse</param>
        /// <param name="joints">All joints</param>
        /// <returns>Parsed finger</returns>
        private HandFinger ParseFinger(MediapipeFingers finger, List<HandJoint> joints)
        {
            return new HandFinger(
                joints[(ushort) MediapipeJoints.WRIST],
                joints[(ushort) finger],
                joints[(ushort) finger + 1],
                joints[(ushort) finger + 2],
                joints[(ushort) finger + 3]
            );
        }

        private float ParseFloat(string v)
        {
            try
            {
                return float.Parse(v, CultureInfo.InvariantCulture.NumberFormat);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        enum MediapipeFingers : ushort
        {
            THUMB = MediapipeJoints.THUMB_CMC,
            INDEX = MediapipeJoints.INDEX_FINGER_MCP,
            MIDDLE = MediapipeJoints.MIDDLE_FINGER_MCP,
            RING = MediapipeJoints.RING_FINGER_MCP,
            PINKY = MediapipeJoints.PINKY_MCP
        }

        enum MediapipeJoints
        {
            WRIST,
            THUMB_CMC, THUMB_MCP, THUMB_IP, THUMB_TIP,
            INDEX_FINGER_MCP, INDEX_FINGER_PIP, INDEX_FINGER_DIP, INDEX_FINGER_TIP,
            MIDDLE_FINGER_MCP, MIDDLE_FINGER_PIP, MIDDLE_FINGER_DIP, MIDDLE_FINGER_TIP,
            RING_FINGER_MCP, RING_FINGER_PIP, RING_FINGER_DIP, RING_FINGER_TIP,
            PINKY_MCP, PINKY_PIP, PINKY_DIP, PINKY_TIP
        }
    }
}