﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kubs
{
    public class Decoder : MonoBehaviour
    {
        public int lastSolutionCount;

        [SerializeField] private Character _character;
        private List<ProgramBlock> listForLoopStart;
        private IEnumerator blockEnumerator;
        private bool reset = false;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        void OnEnable()
        {
            EventManager.StartListening(Constant.EVENT_NAME_CHARACTER_DID_START, SetCharacter);
        }

        void OnDisable()
        {
            EventManager.StopListening(Constant.EVENT_NAME_CHARACTER_DID_START, SetCharacter);
        }

        //Called after program blocks are encoded
        //Calls Character api
        //Decodes the program blocks that were encoded according to the type of program block
        public bool Decode(List<ProgramBlock> blockchain)
        {
            lastSolutionCount = blockchain.Count(p => p.Type != ProgramBlockType.ForLoopEnd);

            listForLoopStart  = new List<ProgramBlock>();
            blockEnumerator = blockchain.GetEnumerator();
            try
            {
                //blockchain should not be modified, added or remove in order for blockEnumberator to work.
                while (blockEnumerator.MoveNext())
                {
                    //Checks if reset was called
                    if (reset)
                    {
                        //Checks if list is null or empty
                        //Sets reset to false if the current enumerator is at the desired for loop
                        if (listForLoopStart != null && listForLoopStart.Count > 0)
                        {
                            int latestForwardLoopIndex = listForLoopStart.Count;
                            ProgramBlock latestForwardLoopBlock = (ProgramBlock)listForLoopStart[latestForwardLoopIndex - 1];
                            if (blockEnumerator.Current.Equals(latestForwardLoopBlock))
                            {
                                reset = false;
                            }
                        }
                    }
                    else
                    {
                        ProgramBlock block = (ProgramBlock)blockEnumerator.Current;
                        if (block.Type == ProgramBlockType.Forward)
                        {
                            //Debug.Log("Decode Character Forward");
                            _character.Forward();
                        }
                        else if (block.Type == ProgramBlockType.RotateLeft)
                        {
                            //Debug.Log("Decode Character Rotate Left");
                            _character.RotateLeft();
                        }
                        else if (block.Type == ProgramBlockType.RotateRight)
                        {
                            //Debug.Log("Decode Character Rotate Right");
                            _character.RotateRight();
                        }
                        else if (block.Type == ProgramBlockType.Jump)
                        {
                            //Debug.Log("Decode Character Jump");
                            _character.Jump();
                        }
                        else if (block.Type == ProgramBlockType.ForLoopStart)
                        {
                            //++forLoopCounter;
                            ProgramBlock tempBlock = block;
                            listForLoopStart.Add(tempBlock);
                        }
                        else if (block.Type == ProgramBlockType.ForLoopEnd)
                        {
                            //--forLoopCounter;
                            int latestForwardLoopIndex = listForLoopStart.Count;
                            --listForLoopStart[latestForwardLoopIndex - 1].Value;
                            if (listForLoopStart[latestForwardLoopIndex - 1].Value > 0)
                            {
                                reset = true;
                                blockEnumerator.Reset();
                            }
                            else
                            {
                                listForLoopStart.RemoveAt(latestForwardLoopIndex - 1);
                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException("BlockChain is modified, iterator not functioning!", e);
            }
            return true;
        }

        private void SetCharacter(object character)
        {
            _character = (Character)character;
        }
    }
}
