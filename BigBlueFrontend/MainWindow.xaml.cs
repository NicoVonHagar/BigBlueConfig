using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Speech.Recognition;
using System.Xml;

namespace BigBlue
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BigBlueWindow
    {
        Brush yellowBrush = new SolidColorBrush(Color.FromRgb(255, 204, 0));
        SolidColorBrush blueBrush = new SolidColorBrush(Color.FromRgb(15, 34, 139));
        Brush transparentBlueBrush = new SolidColorBrush(Color.FromArgb(229, 15, 34, 139));

        int knockOutFrameToDisplay = 0;
        int streetFighterEdition = 1;
        int streetFighterEditionAnimation = 1;
        int timeCode = -1;
        int newChallengerFrameToDisplay = 0;
        int spectatorsFrameToDisplay = 0;
        int spectatorsCurrentFrame = 0;
        int fighterFrameToDisplay = 0;
        int fighterCurrentFrame = 0;
        int rampageFrameToDisplay = 0;
        int rampageCurrentFrame = 0;
        int rtypeFrameToDisplay = 0;
        int rtypeCurrentFrame = 0;
        int haggarFrameToDisplay = 0;
        int igniteFrameToDisplay = 0;
        int screenShakeStartRange = 0;
        int screenShakeEndRange = 0;
        int roundDuration = 99;
        int champion = 0;
        int temporaryChampion = 0;
        int fuseFrameToDisplay = 0;
        int rampageCharacterChoice = 0;
        int haggerSpeed = 0;
        int chunksToCheck = 0;
        int chunksPerTick = 1;
        int staticFrameToDisplay = 0;

        string lastHour1 = string.Empty;
        string lastHour2 = string.Empty;
        string lastMinute1 = string.Empty;
        string lastMinute2 = string.Empty;
        string lastMeridiem = string.Empty;
        string numberType = "0";
        string warpSpeakerPosition;
        string currentDigitOne;
        string currentDigitTwo;
        
        bool fastMusic = false;
        bool decidingWinner = false;
        bool decisionMade = false;
        bool fighterCombo = false;
        bool challengerIntroRunning = false;
        bool screenShaking = false;
        bool snapshotFlicker = false;
        bool freeForAll = false;
        bool fighterAttacking = false;
        bool georgeLeftPunch = false;
        bool georgeRightPunch = false;
        bool climbingUp = true;
        bool rtypeLaserFired = false;
        bool rtypeDestroyed = false;
        bool challengerJoined = false;
        bool roundInProgress = false;
        bool dynamicTimeOfDay = true;
        bool celebratingVictory = false;
        bool showWinnerText = false;
        bool haggarInDanger = false;
        bool waitingForDeathAnimations = false;
        bool spectatorForward = true;

        double numberOfBuildingChunks;
        double totalBuildingsWidth = 1200;
        double currentRampageTopPosition;
        double lifeBarWidth;
        double lowHealthWidth;
        double rtypeLaserWidth;
        double rtypeLaserHeight;
        double rtypeWidth;
        double rtypePlasmaOffset;
        double climbingDownPoint;
        double climbingUpPoint;
        double georgeCollision1Offset;
        double georgeCollision2Offset;
        double georgeCollision3Offset;
        double georgeBodyCollisionOffset;
        double damageDecalYCenter = 0;
        double rtypeCollisionBoxVerticalPosition;
        double rtypeLaserVerticalPosition;
        double rtypeShipOnlyLength;
        double rtypeVerticalPosition;
        double bushHeight;
        double knifeTableOffset = 0;
        double chunkHeight;

        internal override void BeginGameLaunch()
        {
            base.BeginGameLaunch();

            // start the fighter punch sequence
            fighterFrameToDisplay = 3;
        }
        
        private void PlayerMenuShortcutAction(string action, Models.FrontEndExitMode em, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (frontendInputs[action].wasPressed == true)
                {
                    frontendInputs[action].isRepeating = true;
                }

                if (!menuOpen && shutdownSequenceActivated == true && (haggarInDanger == true || shutdown == true))
                {
                    exitMode = em;
                    ExitFrontEnd(FrontEndContainer);
                    // unpress all other keys here
                    return;
                }

                if (!menuOpen && !itsGoTime && !screenSaver && !frontendInputs[action].isRepeating && !haggarInDanger && !shutdown)
                {
                    exitMode = em;
                    StartShutdown(false);
                }

                frontendInputs[action].wasPressed = true;
            }
            else
            {
                ReleaseInput(action);
            }
        }
        
        private async void PlayerStartAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                frontendInputs[action].wasPressed = true;

                if (itsGoTime == false && screenSaver == false && shutdown == false && frontendInputs[action].isRepeating == false)
                {
                    if ((champion == 0 && action == "RAMPAGE_START") || (champion == 1 && action == "RTYPE_START") || freeForAll == true)
                    {
                        if (menuOpen == true)
                        {
                            switch (selectedMenuIndex)
                            {
                                case 0:
                                    ToggleMenu();
                                    break;
                                case 1:
                                    if (displayExitItemInMenu == true)
                                    {
                                        exitMode = Models.FrontEndExitMode.quit;
                                    }
                                    else
                                    {
                                        exitMode = BigBlue.Models.FrontEndExitMode.shutdown;
                                    }
                                    StartShutdown(true);
                                    break;
                                case 2:
                                    if (displayExitItemInMenu == true)
                                    {
                                        exitMode = BigBlue.Models.FrontEndExitMode.shutdown;
                                    }
                                    else
                                    {
                                        exitMode = BigBlue.Models.FrontEndExitMode.restart;
                                    }
                                    StartShutdown(true);
                                    break;
                                case 3:
                                    exitMode = BigBlue.Models.FrontEndExitMode.restart;
                                    StartShutdown(true);
                                    break;
                            }
                        }
                        else
                        {
                            if (roundInProgress == false && challengerIntroRunning == false && awaitingAsync == false && itsGoTime == false)
                            {
                                if (!string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder))
                                {
                                    BigBlue.XAudio2Player.PlaySound(SelectListSoundKey, string.Empty);

                                    bool returnToOriginatingList = false;

                                    string childFolderName = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder;


                                    if (!string.IsNullOrWhiteSpace(childFolderName))
                                    {
                                        subFolderTrail.Add(childFolderName);
                                    }                              
                                    
                                    if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID == null)
                                    {
                                        awaitingAsync = true;
                                        SetGameSnapshots(true);
                                        
                                        GameList.Visibility = Visibility.Hidden;

                                        bool validList = await Task.Run(() => { return OpenSubFolderListItem(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildFolder, folderListGuid); });
                                        
                                        if (validList)
                                        {
                                            RenderSubFolderList(videoMe, videoCanvas);
                                        }
                                        else
                                        {
                                            returnToOriginatingList = true;
                                        }

                                        awaitingAsync = false;
                                        GameList.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;
                                        
                                        Guid lg = (Guid)frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID;

                                        originatingFolderParentListItem = frontendLists[selectedListGuid].ListItems[selectedListItemIndex];
                                        originatingFolderListGuid = lg;

                                        awaitingAsync = true;
                                        SetGameSnapshots(true);
                                        GameList.Visibility = Visibility.Hidden;

                                        bool validList = await Task.Run(() => { return OpenFolderListItem(lg); });

                                        if (validList)
                                        {
                                            RenderFolderList(lg);
                                        }
                                        else
                                        {
                                            returnToOriginatingList = true;
                                        }

                                        awaitingAsync = false;
                                        GameList.Visibility = Visibility.Visible;
                                    }

                                    if (returnToOriginatingList)
                                    {
                                        subFolderTrail.Clear();
                                        
                                        if (frontendLists[originatingFolderListGuid].ListItems != null)
                                        {
                                            // can't do anything if there are 0 items
                                            if (frontendLists[originatingFolderListGuid].ListItems.Count > 0)
                                            {
                                                Guid? fallbackGuid = frontendLists[originatingFolderListGuid].ListItems[0].ParentID;

                                                if (fallbackGuid != null)
                                                {
                                                    SelectList((Guid)fallbackGuid, false);
                                                }
                                            }
                                            else
                                            {
                                                // this is a last resort in case something really horrible goes wrong, but the count should always be greater than 0
                                                SelectList(selectedListGuid, false);
                                            }
                                        }
                                    }
                                }
                                else if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID != null)
                                {
                                    BigBlue.XAudio2Player.PlaySound(SelectListSoundKey, string.Empty);

                                    frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;

                                    SelectList((Guid)frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ChildID, false);
                                }
                                else
                                {
                                    if (ValidateProgramLaunch())
                                    {
                                        itsGoTime = true;
                                        ReleaseAllInputs();

                                        // disable inputs if you've got 'em
                                        if (launchInputDelay > 0)
                                        {
                                            DisableKeys(this);
                                        }

                                        // bring rtype ship out of warp
                                        RtypeDropWarp();
                                        
                                        // stop the combo
                                        if (fighterCombo == true || fighterAttacking == true)
                                        {
                                            fighterCombo = false;
                                            fighterAttacking = false;
                                        }

                                        // hide the game list
                                        GameList.Visibility = System.Windows.Visibility.Hidden;

                                        if (freeForAll == false)
                                        {
                                            PressStartRectangle.Visibility = System.Windows.Visibility.Collapsed;
                                        }

                                        // play the select sound
                                        VideoElement.Volume = 0;

                                        // you need to wait for at least some of the sound to be played                                         
                                        BigBlue.XAudio2Player.StopAllSounds();
                                        BigBlue.XAudio2Player.PlaySound(LaunchListItemSoundKey, null);
                                        
                                        if (BigBlue.XAudio2Player.Disabled)
                                        {
                                            awaitingAsync = true;

                                            await Task.Run(() =>
                                            {
                                                System.Threading.Thread.Sleep(500);
                                            });

                                            awaitingAsync = false;

                                            BeginGameLaunch();
                                        }
                                    }
                                    else
                                    {
                                        awaitingAsync = true;
                                        PlayLoseSound();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (freeForAll == false && itsGoTime == false && menuOpen == false && pausedBySystem == false && roundInProgress == false && awaitingAsync == false && challengerIntroRunning == false && decisionMade == false && ((champion == 1 && action == "RAMPAGE_START") || (champion == 0 && action == "RTYPE_START")))
                        {
                            HereComesANewChallenger();
                        }
                    }
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }

        
        private void InitiateSpeechAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                frontendInputs[action].wasPressed = true;

                if (roundInProgress == false && challengerIntroRunning == false && awaitingAsync == false && itsGoTime == false && screenSaver == false && shutdown == false && frontendInputs[action].isRepeating == false && recognizer != null)
                {
                    //KeyTest.Text = string.Empty;
                    MatchingSearchWords.Clear();
                    recognizer.RecognizeAsyncCancel();
                    
                    //searchCommand.clear = string.Empty;

                    awaitingAsync = true;
                    GameList.Visibility = Visibility.Hidden;

                    StopVideo();

                    // set all the images to the speech thumbnail
                    SnapShotRectangle.Source = speechThumbnailImage;

                    if (marqueeDisplay)
                    {
                        marqueeWindow.SecondaryWindowSnapshot.Source = speechThumbnailImage;
                    }

                    if (flyerDisplay)
                    {
                        flyerWindow.SecondaryWindowSnapshot.Source = speechThumbnailImage;
                    }

                    if (instructionDisplay)
                    {
                        instructionWindow.SecondaryWindowSnapshot.Source = speechThumbnailImage;
                    }

                    // recognize the speech
                    recognizer.RecognizeAsync(RecognizeMode.Single);
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }
        
        private bool PressInput(string action)
        {
            if (frontendInputs[action].wasPressed == true)
            {
                frontendInputs[action].isRepeating = true;
            }

            if (InterruptVersusScreen() == true || InterruptShutDownSequence() == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private async void GameListBackAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && shutdown == false && roundInProgress == false && challengerIntroRunning == false && menuOpen == false && awaitingAsync == false && itsGoTime == false && inputDown == true && (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentID != null || !string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentFolder) || selectedListGuid == searchListGuid) && ((champion == 0 && action == "RAMPAGE_BACK") || (champion == 1 && action == "RTYPE_BACK")))
                {
                    BigBlue.XAudio2Player.PlaySound(ExitListSoundKey, null);

                    string originalParentFolder = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentFolder;

                    if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentID != null)
                    {
                        subFolderTrail.Clear();

                        Guid parentGuid = (Guid)frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentID;

                        // when we're going back, let's set the current index of the list to be whatever it was at that time
                        frontendLists[selectedListGuid].CurrentListIndex = selectedListItemIndex;


                        selectedListItemIndex = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].IndexOfParent;

                        SelectList(parentGuid, true);
                    }
                    else
                    {
                        //!string.IsNullOrWhiteSpace(originalParentFolder) && 
                        if (subFolderTrail.Count > 0)
                        {
                            subFolderTrail.RemoveAt(subFolderTrail.Count - 1);
                        }

                        bool returnToOriginatingList = false;
                        
                        if (selectedListGuid == searchListGuid)
                        {
                            returnToOriginatingList = true;
                        }

                        //if (!string.IsNullOrWhiteSpace(frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentFolder))
                        if (subFolderTrail.Count > 0 && !returnToOriginatingList)
                        {
                            //frontendLists[selectedListGuid].ListItems[selectedListItemIndex].ParentFolder
                                                        
                            awaitingAsync = true;
                            GameList.Visibility = Visibility.Hidden;
                            SetGameSnapshots(true);

                            bool validList = await Task.Run(() => { return OpenSubFolderListItem(subFolderTrail[subFolderTrail.Count - 1], folderListGuid); });

                           // subFolderTrail.RemoveAt(subFolderTrail.Count - 1);
                           
                            if (validList)
                            {
                                RenderSubFolderList(videoMe, videoCanvas);
                            }
                            else
                            {
                                returnToOriginatingList = true;
                                //subFolderTrail.RemoveAt(subFolderTrail.Count - 1);
                                // this bullcrap needs to check to make sure that the originating folder list even still exists, and if it doesn't, grab its parent
                                
                            }

                            awaitingAsync = false;
                            GameList.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            returnToOriginatingList = true;
                            //selectList(originatingFolderListGuid);
                        }
                        
                        if (returnToOriginatingList)
                        {
                            subFolderTrail.Clear();
                            
                            if (originatingFolderListGuid == Guid.Empty || selectedListGuid == searchListGuid)
                            {
                                selectedListItemIndex = frontendLists[rootListGuid].CurrentListIndex;
                                SelectList(rootListGuid, true);
                            }
                            else
                            {
                                Guid? fallbackGuid = frontendLists[originatingFolderListGuid].ListItems[0].ParentID;

                                if (fallbackGuid != null)
                                {
                                    // when we're going back, let's set the current index of the list to be whatever it was at that time
                                   // frontendLists[(Guid)fallbackGuid].CurrentListIndex = selectedListItemIndex;

                                    //selectedListItemIndex = frontendLists[selectedListGuid].ListItems[selectedListItemIndex].IndexOfParent;

                                    selectedListItemIndex = frontendLists[(Guid)fallbackGuid].CurrentListIndex;

                                    SelectList((Guid)fallbackGuid, true);
                                }
                            }
                        }
                    }    
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }

        //async
        private void MenuAction(string action, bool? inputDown)
        {
            // shouldn't be able to use the menu while a game is starting
            if (inputDown == true)
            {
                if (frontendInputs[action].wasPressed == true)
                {
                    frontendInputs[action].isRepeating = true;
                }

                if (frontendInputs[action].isRepeating == false && (action == "BIG_BLUE_EXIT" || (champion == 0 && action == "RAMPAGE_EXIT") || (champion == 1 && action == "RTYPE_EXIT")))
                {
                    if (FrontEndContainer.Opacity == 1 && screenSaver == false && itsGoTime == false && awaitingAsync == false)
                    {
                        if (!disableMenu)
                        {
                            if (shutdownSequenceActivated == true && (haggarInDanger == true || shutdown == true))
                            {
                                ExitFrontEnd(FrontEndContainer);
                                // unpress all other keys here
                                return;
                            }

                            ToggleMenu();
                        }
                    }
                    else
                    {
                        if (globalInputs == true && frontendLists[selectedListGuid].ListItems[selectedListItemIndex].KillTask == true && itsGoTime == true)
                        {
                            BigBlue.ProcessHandling.CloseAllProcesses(processes);
                            
                            //processName = string.Empty;    
                            //processId = -1;

                            ReturnFromGame(FrontEndContainer);
                        }
                    }
                }

                frontendInputs[action].wasPressed = true;
            }
            else
            {
                ReleaseInput(action);
            }
        }

        private void RtypeShootAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                double rtypeRightPosition = currentRtypeLeftPosition + rtypeWidth;

                if (rtypeRightPosition >= 0 && rtypeRightPosition <= width && Rtype.Fill != rtypeAnimationFrames[timeCode][1] && frontendInputs[action].isRepeating == false && rtypeShipState == RtypeState.flying && screenSaver == false && celebratingVictory == false && itsGoTime == false && shutdown == false && challengerIntroRunning == false && pausedBySystem == false && rtypeDestroyed == false && menuOpen == false)
                {
                    rtypeLaserFired = true;
                }
                else
                {
                    rtypeLaserFired = false;
                }

                frontendInputs[action].wasPressed = true;
            }
            else
            {
                ReleaseInput(action);
            }
        }

        private void RtypeReturnToNormalSpeed()
        {
            warpStopWatch.Reset();
            rtypeShipState = RtypeState.flying;

            // you only want to change the speed ratio if it's at the wrong speed
            RtypeFlyingStoryboard.SetSpeedRatio(horizontalAnimationSpeedRatio);
        }

        private void RtypeWarpAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && itsGoTime == false && shutdown == false && menuOpen == false && challengerIntroRunning == false && pausedBySystem == false && rtypeDestroyed == false && rtypeShipState != RtypeState.dead && menuOpen == false && frontendInputs[action].isRepeating == false)
                {
                    rtypeShipState = RtypeState.warping;

                    BigBlue.XAudio2Player.PlaySound("warp", GetRtypeSpeakerPosition(currentRtypeLeftPosition));

                    // you only want to change the speed ratio if it's at the wrong speed
                    RtypeFlyingStoryboard.SetSpeedRatio(horizontalAnimationSpeedRatio * RTYPE_WARP_FACTOR);
                }

                frontendInputs[action].wasPressed = true;
            }
            else
            {
                RtypeDropWarp();

                ReleaseInput(action);
            }
        }

        private void RtypeDropWarp()
        {
            if (rtypeShipState == RtypeState.warping)
            {
                // stop looping the sound when this happens
                BigBlue.XAudio2Player.StopSound("warp");

                if (rtypeDestroyed == false && rtypeShipState != RtypeState.dead)
                {
                    RtypeReturnToNormalSpeed();
                }
            }
        }

        private void PaintCollisionBox(Rect collisionBox)
        {
            Rectangle collisionRectangle = new Rectangle();
            collisionRectangle.Fill = blueBrush;

            collisionRectangle.Width = collisionBox.Width;
            collisionRectangle.Height = collisionBox.Height;

            FrontEndContainer.Children.Add(collisionRectangle);

            Canvas.SetLeft(collisionRectangle, collisionBox.X);
            Canvas.SetTop(collisionRectangle, collisionBox.Y);
        }

        private void RampagePunchLeftAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && pausedBySystem == false && challengerIntroRunning == false && shutdown == false && itsGoTime == false && menuOpen == false && (georgeState == RampageState.climbing || georgeState == RampageState.punchingDown || georgeState == RampageState.punchingLeft || georgeState == RampageState.punchingRight))
                {
                    if (frontendInputs[action].isRepeating == true)
                    {
                        georgeLeftPunch = false;
                    }
                    else
                    {
                        if (RampageMonsterRectangle.Fill == rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][4] || RampageMonsterRectangle.Fill == rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][6] || RampageMonsterRectangle.Fill == rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][5])
                        {
                            //georgePunching = false;
                            georgeLeftPunch = false;
                        }
                        else
                        {
                            //georgePunching = true;
                            georgeLeftPunch = true;
                            frontendInputs[action].wasPressed = true;
                        }
                    }
                }
                else
                {
                    //georgePunching = false;
                    georgeRightPunch = false;
                    georgeLeftPunch = false;
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }

        private void RampagePunchRightAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && pausedBySystem == false && challengerIntroRunning == false && shutdown == false && itsGoTime == false && menuOpen == false && (georgeState == RampageState.climbing || georgeState == RampageState.punchingDown || georgeState == RampageState.punchingLeft || georgeState == RampageState.punchingRight))
                {
                    if (frontendInputs[action].isRepeating == true)
                    {
                        georgeRightPunch = false;
                    }
                    else
                    {
                        // here we're going to handle the key press when george is already punching right
                        //if (RampageImage.Viewbox == georgeRects[6] || RampageImage.Viewbox == georgeRects[5] || RampageImage.Viewbox == georgeRects[4])
                        if (RampageMonsterRectangle.Fill == rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][6] || RampageMonsterRectangle.Fill == rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][5] || RampageMonsterRectangle.Fill == rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][4])
                        {
                            //georgePunching = false;
                            georgeRightPunch = false;
                        }
                        else
                        {
                            //georgePunching = true;
                            georgeRightPunch = true;
                            frontendInputs[action].wasPressed = true;
                        }
                    }
                }
                else
                {
                    //georgePunching = false;
                    georgeRightPunch = false;
                    georgeLeftPunch = false;
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }

        private void RampageSpectateAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && pausedBySystem == false && challengerIntroRunning == false && menuOpen == false && shutdown == false && itsGoTime == false && georgeState != RampageState.dead && georgeState != RampageState.falling && georgeState != RampageState.burning)
                {
                    fighterCombo = true;

                    if (fighterAttacking == false)
                    {
                        fighterAttacking = true;
                    }
                }
            }
            else
            {
                ReleaseInput(action);
                fighterCombo = false;
                fighterAttacking = false;
            }
        }

        private void PreviousItemKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && shutdown == false && awaitingAsync == false && itsGoTime == false && ((champion == 0 && action == "RAMPAGE_PREVIOUS_ITEM") || (champion == 1 && action == "RTYPE_PREVIOUS_ITEM")) && frontendInputs[action].isRepeating == false)
                {
                    // might not need to manually release the other keys
                    frontendInputs["RAMPAGE_NEXT_ITEM"].wasPressed = false;
                    frontendInputs["RAMPAGE_PREVIOUS_PAGE"].wasPressed = false;
                    frontendInputs["RAMPAGE_NEXT_PAGE"].wasPressed = false;

                    frontendInputs["RTYPE_NEXT_ITEM"].wasPressed = false;
                    frontendInputs["RTYPE_PREVIOUS_PAGE"].wasPressed = false;
                    frontendInputs["RTYPE_NEXT_PAGE"].wasPressed = false;

                    if (menuOpen == true)
                    {
                        CalculatePreviousMenuItem(MainMenu, yellowBrush, whiteBrush);
                    }
                    else
                    {
                        if (roundInProgress == false && challengerIntroRunning == false)
                        {
                            CalculateGame(-1);
                            //calculatePreviousGame();
                        }
                    }

                    frontendInputs[action].wasPressed = true;
                }
            }
            else
            {
                ReleaseInput(action);
                timeToChange = 400;
            }
        }

        private void RandomItemKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (menuOpen == false && screenSaver == false && shutdown == false && awaitingAsync == false && itsGoTime == false && ((champion == 0 && action == "RAMPAGE_RANDOM_ITEM") || (champion == 1 && action == "RTYPE_RANDOM_ITEM")) && frontendInputs[action].isRepeating == false)
                {
                    if (roundInProgress == false && challengerIntroRunning == false)
                    {
                        // generate a random number between 0 and the totalList amount
                        selectedListItemIndex = r.Next(0, frontendLists[selectedListGuid].Total + 1);

                        if (RenderGameListCheck() == true)
                        {
                            RenderGameList(false);
                        }

                        if (action == "RAMPAGE_RANDOM_ITEM")
                        {
                            PlayerStartAction("RAMPAGE_START", inputDown);
                        }
                        else
                        {
                            PlayerStartAction("RTYPE_START", inputDown);
                        }
                    }

                    frontendInputs[action].wasPressed = true;
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }

        private void FirstItemKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && shutdown == false && awaitingAsync == false && itsGoTime == false && ((champion == 0 && action == "RAMPAGE_FIRST_ITEM") || (champion == 1 && action == "RTYPE_FIRST_ITEM")) && frontendInputs[action].isRepeating == false)
                {
                    if (roundInProgress == false && challengerIntroRunning == false)
                    {
                        selectedListItemIndex = 0;

                        if (RenderGameListCheck() == true)
                        {
                            RenderGameList(true);
                        }
                    }

                    frontendInputs[action].wasPressed = true;
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }

        private void LastItemKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && shutdown == false && awaitingAsync == false && itsGoTime == false && ((champion == 0 && action == "RAMPAGE_LAST_ITEM") || (champion == 1 && action == "RTYPE_LAST_ITEM")) && frontendInputs[action].isRepeating == false)
                {
                    if (roundInProgress == false && challengerIntroRunning == false)
                    {
                        selectedListItemIndex = frontendLists[selectedListGuid].Total;

                        if (RenderGameListCheck() == true)
                        {
                            RenderGameList(true);
                        }
                    }

                    frontendInputs[action].wasPressed = true;
                }
            }
            else
            {
                ReleaseInput(action);
            }
        }

        private void NextItemKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && shutdown == false && awaitingAsync == false && itsGoTime == false && ((champion == 0 && action == "RAMPAGE_NEXT_ITEM") || (champion == 1 && action == "RTYPE_NEXT_ITEM")) && frontendInputs[action].isRepeating == false)
                {
                    frontendInputs["RAMPAGE_PREVIOUS_PAGE"].wasPressed = false;
                    frontendInputs["RTYPE_PREVIOUS_PAGE"].wasPressed = false;
                    frontendInputs["RAMPAGE_PREVIOUS_ITEM"].wasPressed = false;
                    frontendInputs["RTYPE_PREVIOUS_ITEM"].wasPressed = false;
                    frontendInputs["RAMPAGE_NEXT_PAGE"].wasPressed = false;
                    frontendInputs["RTYPE_NEXT_PAGE"].wasPressed = false;

                    frontendInputs[action].wasPressed = true;

                    if (menuOpen == true)
                    {
                        CalculateNextMenuItem(MainMenu, yellowBrush, whiteBrush);
                    }
                    else
                    {
                        if (roundInProgress == false && challengerIntroRunning == false)
                        {
                            CalculateGame(1);
                        }
                    }
                }
            }
            else
            {
                ReleaseInput(action);
                timeToChange = 400;
            }
        }

        private void GameListMouseAction(string action)
        {
            if (mouseStopWatch.ElapsedMilliseconds > mouseMovementSpeed)
            {
                if (InterruptVersusScreen() == true || InterruptShutDownSequence() == true)
                {
                    return;
                }

                if (screenSaver == false && shutdown == false && awaitingAsync == false && itsGoTime == false)
                {
                    if (menuOpen == true)
                    {
                        if ((champion == 0 && action == "RAMPAGE_NEXT_ITEM") || (champion == 1 && action == "RTYPE_NEXT_ITEM"))
                        {
                            CalculateNextMenuItem(MainMenu, yellowBrush, whiteBrush);
                        }

                        if ((champion == 0 && action == "RAMPAGE_PREVIOUS_ITEM") || (champion == 1 && action == "RTYPE_PREVIOUS_ITEM"))
                        {
                            CalculatePreviousMenuItem(MainMenu, yellowBrush, whiteBrush);
                        }
                    }
                    else
                    {
                        if (roundInProgress == false && challengerIntroRunning == false && pausedBySystem == false)
                        {
                            if ((champion == 0 && action == "RAMPAGE_NEXT_ITEM") || (champion == 1 && action == "RTYPE_NEXT_ITEM"))
                            {
                                CalculateGame(1);
                            }

                            if ((champion == 0 && action == "RAMPAGE_PREVIOUS_ITEM") || (champion == 1 && action == "RTYPE_PREVIOUS_ITEM"))
                            {
                                CalculateGame(-1);
                            }

                            if ((champion == 0 && action == "RAMPAGE_PREVIOUS_PAGE") || (champion == 1 && action == "RTYPE_PREVIOUS_PAGE"))
                            {
                                CalculateGame(-frontendLists[selectedListGuid].TextBlockItemsToPage);
                            }

                            if ((champion == 0 && action == "RAMPAGE_NEXT_PAGE") || (champion == 1 && action == "RTYPE_NEXT_PAGE"))
                            {
                                CalculateGame(frontendLists[selectedListGuid].TextBlockItemsToPage);
                            }
                        }
                    }
                }
                // this should be moved into the mouse methods and you should return after getting one
                mouseStopWatch.Restart();
            }
        }

        private void PreviousPageKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && pausedBySystem == false && shutdown == false && awaitingAsync == false && itsGoTime == false && roundInProgress == false && challengerIntroRunning == false && ((champion == 0 && action == "RAMPAGE_PREVIOUS_PAGE") || (champion == 1 && action == "RTYPE_PREVIOUS_PAGE")) && menuOpen == false && frontendInputs[action].isRepeating == false)
                {
                    frontendInputs["RAMPAGE_NEXT_ITEM"].wasPressed = false;
                    frontendInputs["RTYPE_NEXT_ITEM"].wasPressed = false;
                    frontendInputs["RAMPAGE_PREVIOUS_ITEM"].wasPressed = false;
                    frontendInputs["RTYPE_PREVIOUS_ITEM"].wasPressed = false;
                    frontendInputs["RAMPAGE_NEXT_PAGE"].wasPressed = false;
                    frontendInputs["RTYPE_NEXT_PAGE"].wasPressed = false;

                    frontendInputs[action].wasPressed = true;

                    CalculateGame(-frontendLists[selectedListGuid].TextBlockItemsToPage);
                }
            }
            else
            {
                ReleaseInput(action);
                timeToChange = 400;
            }
        }

        private void NextPageKeyboardAction(string action, bool? inputDown)
        {
            if (inputDown == true)
            {
                if (PressInput(action) == true)
                {
                    return;
                }

                if (screenSaver == false && pausedBySystem == false && shutdown == false && awaitingAsync == false && itsGoTime == false && roundInProgress == false && challengerIntroRunning == false && ((champion == 0 && action == "RAMPAGE_NEXT_PAGE") || (champion == 1 && action == "RTYPE_NEXT_PAGE")) && menuOpen == false && frontendInputs[action].isRepeating == false)
                {
                    frontendInputs["RAMPAGE_NEXT_ITEM"].wasPressed = false;
                    frontendInputs["RTYPE_NEXT_ITEM"].wasPressed = false;
                    frontendInputs["RAMPAGE_PREVIOUS_ITEM"].wasPressed = false;
                    frontendInputs["RTYPE_PREVIOUS_ITEM"].wasPressed = false;
                    frontendInputs["RAMPAGE_PREVIOUS_PAGE"].wasPressed = false;
                    frontendInputs["RTYPE_PREVIOUS_PAGE"].wasPressed = false;

                    frontendInputs[action].wasPressed = true;

                    if (listTypePriority == Models.ListType.Image)
                    {
                        CalculateGame(frontendLists[selectedListGuid].ImageItemsToPage);
                    }
                    else
                    {
                        CalculateGame(frontendLists[selectedListGuid].TextBlockItemsToPage);
                    }
                }
            }
            else
            {
                ReleaseInput(action);
                timeToChange = 400;
            }
        }

        internal override void ProcessScreenSaverInput()
        {
            // for each key press, you need to reset the screen saver timer so it doesn't keep going to sleep
            if (screenSaverTimeInMinutes >= 1)
            {
                screenSaverTimer.Restart();
            }

            if (screenSaver == true)
            {
                // if the screen saver was on during one of the time transitions, we'll recalculate
                CalculateTime(true);
                ChooseRampageCharacter(false);

                if (menuOpen == false)
                {
                    ResumeFrontend();
                }

                screenSaver = false;

                //fadeEffect(1);

                ScreensaverEndStoryboard.Begin();
            }
        }

        private bool InterruptShutDownSequence()
        {
            // any input can interrupt the shutdown sequence except the exit key
            if ((shutdownSequenceActivated == true || haggarInDanger == true || shutdown == true) && pausedBySystem == false)
            {
                if (canThrowKnife == true)
                {
                    KnifeStoryboard.Begin();
                    canThrowKnife = false;
                }

                return true;
            }

            return false;
        }

        private bool InterruptVersusScreen()
        {
            if (!freeForAll && !menuOpen)
            {
                if (XAudio2Player.StopSound("versus"))
                {
                    HideVersusScreen();
                    return true;
                }
            }
            
            return false;
        }
                        
        Brush transparentBrush = new SolidColorBrush(Colors.Transparent);
        
        Brush morningBrush = new SolidColorBrush(Color.FromRgb(149, 211, 211));
        Brush sunsetBrush = new SolidColorBrush(Color.FromRgb(170, 68, 0));
        Brush nightBrush = new SolidColorBrush(Color.FromRgb(17, 51, 102));

        Dictionary<Guid, Storyboard> laserStoryboards = new Dictionary<Guid, Storyboard>();
        Dictionary<Guid, Rectangle> laserBeams = new Dictionary<Guid, Rectangle>();

        List<Point> fusePoints = new List<Point>();

        List<Rectangle> buildingBlockDecals;
        List<Rectangle> buildingBlocks;
        List<Rectangle> buildingBlocksRight = new List<Rectangle>();
        List<Rectangle> buildingDebris;

        List<Rect> koRects = new List<Rect>(1);
        List<Rect> winnerTextRects = new List<Rect>(7);
        List<Rect> buildingRects = new List<Rect>();

        List<ImageBrush> fighterBloodBrushes = new List<ImageBrush>(4);
        
        Storyboard RematchStartStoryboard = new Storyboard();
        Storyboard RematchEndStoryboard = new Storyboard();
        
        //Storyboard HideFrontEndStoryboard = new Storyboard();
        Storyboard ShowVersusStoryboard = new Storyboard();
        Storyboard HideVersusStoryboard = new Storyboard();

        Storyboard ScreenStoryBoard = new Storyboard();
        Storyboard SkyStoryBoard = new Storyboard();
        Storyboard Sky2StoryBoard = new Storyboard();
        Storyboard Sky3StoryBoard = new Storyboard();
        Storyboard Sky4StoryBoard = new Storyboard();
        Storyboard Cloud1StoryBoard = new Storyboard();
        Storyboard Cloud2StoryBoard = new Storyboard();
        Storyboard Cloud3StoryBoard = new Storyboard();
        Storyboard Cloud4StoryBoard = new Storyboard();
        Storyboard Cloud5StoryBoard = new System.Windows.Media.Animation.Storyboard();

        Storyboard RtypeFlyingStoryboard = new Storyboard();
        Storyboard KnifeStoryboard = new Storyboard();
        Storyboard GeorgeClimbingStoryBoard = new Storyboard();

        Storyboard SkyGradientMorningStoryBoard1 = new Storyboard();
        Storyboard SkyGradientMorningStoryBoard2 = new Storyboard();
        Storyboard SkyGradientMorningStoryBoard3 = new Storyboard();
        Storyboard SkyGradientMorningStoryBoard4 = new Storyboard();
        Storyboard SkyGradientMorningStoryBoard5 = new Storyboard();

        Storyboard Stippled1MorningStoryBoard = new Storyboard();
        Storyboard Stippled2MorningStoryBoard = new Storyboard();
        Storyboard Stippled3MorningStoryBoard = new Storyboard();
        Storyboard Stippled4MorningStoryBoard = new Storyboard();

        Storyboard SkyGradientSunsetStoryBoard1 = new Storyboard();
        Storyboard SkyGradientSunsetStoryBoard2 = new Storyboard();
        Storyboard SkyGradientSunsetStoryBoard3 = new Storyboard();
        Storyboard SkyGradientSunsetStoryBoard4 = new Storyboard();
        Storyboard SkyGradientSunsetStoryBoard5 = new Storyboard();

        Storyboard Stippled1SunsetStoryBoard = new Storyboard();
        Storyboard Stippled2SunsetStoryBoard = new Storyboard();
        Storyboard Stippled3SunsetStoryBoard = new Storyboard();
        Storyboard Stippled4SunsetStoryBoard = new Storyboard();

        Storyboard SkyGradientNightStoryBoard1 = new Storyboard();
        Storyboard SkyGradientNightStoryBoard2 = new Storyboard();
        Storyboard SkyGradientNightStoryBoard3 = new Storyboard();
        Storyboard SkyGradientNightStoryBoard4 = new Storyboard();
        Storyboard SkyGradientNightStoryBoard5 = new Storyboard();

        Storyboard Stippled1NightStoryBoard = new Storyboard();
        Storyboard Stippled2NightStoryBoard = new Storyboard();
        Storyboard Stippled3NightStoryBoard = new Storyboard();
        Storyboard Stippled4NightStoryBoard = new Storyboard();

        Storyboard SkyGradientDawnStoryBoard1 = new Storyboard();
        Storyboard SkyGradientDawnStoryBoard2 = new Storyboard();
        Storyboard SkyGradientDawnStoryBoard3 = new Storyboard();
        Storyboard SkyGradientDawnStoryBoard4 = new Storyboard();
        Storyboard SkyGradientDawnStoryBoard5 = new Storyboard();

        Storyboard Stippled1DawnStoryBoard = new Storyboard();
        Storyboard Stippled2DawnStoryBoard = new Storyboard();
        Storyboard Stippled3DawnStoryBoard = new Storyboard();
        Storyboard Stippled4DawnStoryBoard = new Storyboard();

        // Create a Storyboard to contain and apply the animations.
        Storyboard fusePathAnimationStoryboard = new Storyboard();

        List<Storyboard> debrisStoryboards = new List<Storyboard>();
        //List<DoubleAnimation> debrisAnimations = new List<DoubleAnimation>();

        DoubleAnimation georgeFallingAnimation = new DoubleAnimation();
        Storyboard GeorgeFallingStoryBoard = new Storyboard();
        
        Stopwatch genericAnimationStopWatch = new Stopwatch();

        Stopwatch roundTimer = new Stopwatch();
        Stopwatch newChallengerTimer = new Stopwatch();
        //Stopwatch georgeTimer = new Stopwatch();

        long rampageElapsedMilliseconds = 0;

        long rtypeElapsedMilliseconds = 0;
        //Stopwatch rtypeTimer = new Stopwatch();

        long fightersElapsedMilliseconds = 0;
        //Stopwatch fightersTimer = new Stopwatch();

        long crowdElapsedMilliseconds = 0;
        //Stopwatch crowdTimer = new Stopwatch();
        //Stopwatch plasmaTimer = new Stopwatch();
        long plasmaElapsedMilliseconds = 0;

        long buildingElapsedMilliseconds = 0;
        //Stopwatch buildingTimer = new Stopwatch();


        //List<Stopwatch> debrisTimers = new List<System.Diagnostics.Stopwatch>();
        List<long> debrisElapsedMilliseconds = new List<long>();

        long userInterfaceElapsedMilliseconds = 0;

        //Stopwatch uiTimer = new Stopwatch();
        Stopwatch spectateStopWatch = new Stopwatch();
        Stopwatch warpStopWatch = new Stopwatch();
        Stopwatch screenShakeTimer = new Stopwatch();
        Stopwatch decisionTimer = new Stopwatch();

        long haggarElapsedMilliseconds = 0;
        //Stopwatch haggarTimer = new Stopwatch();
        //Stopwatch igniteTimer = new Stopwatch();
        long igniteElapsedMilliseconds = 0;

        //Stopwatch staticTimer = new Stopwatch();
        long staticElapsedMilliseconds = 0;
        Stopwatch explosionTimer = new Stopwatch();
        Stopwatch winnerTextTimer = new Stopwatch();

        Stopwatch victoryTimer = new Stopwatch();
        long fuseElapsedMilliseconds = 0;
        //Stopwatch fuseTimer = new Stopwatch();

        private void PauseStopWatches(bool pauseAll)
        {
            genericAnimationStopWatch.Stop();
            //georgeTimer.Stop();
            //rtypeTimer.Stop();
            //fightersTimer.Stop();
            //crowdTimer.Stop();
            //plasmaTimer.Stop();
            //buildingTimer.Stop();

            /*
            foreach (Stopwatch dsw in debrisTimers)
            {
                dsw.Stop();
            }
            */
            
            //uiTimer.Stop();
            spectateStopWatch.Stop();
            warpStopWatch.Stop();

            if (screenShaking == true)
            {
                screenShakeTimer.Stop();
            }

            explosionTimer.Stop();

            //staticTimer.Stop();
            //haggarTimer.Stop();
            //igniteTimer.Stop();
            //fuseTimer.Stop();

            roundTimer.Stop();

            if (challengerJoined == true)
            {
                newChallengerTimer.Stop();
            }

            if (pauseAll)
            {
                winnerTextTimer.Stop();
                decisionTimer.Stop();
            }

            if (celebratingVictory == true)
            {
                victoryTimer.Stop();
            }
        }

        private void ResumeStopWatches(bool resumeAll)
        {
            genericAnimationStopWatch.Start();
            //georgeTimer.Start();
            //rtypeTimer.Start();
            //fightersTimer.Start();
            //crowdTimer.Start();
            //plasmaTimer.Start();
            //buildingTimer.Start();

            /*
            foreach (Stopwatch dsw in debrisTimers)
            {
                dsw.Start();
            }
            */

            //uiTimer.Start();
            spectateStopWatch.Start();

            if (rtypeShipState == RtypeState.warping && warpStopWatch.IsRunning == false)
            {
                warpStopWatch.Start();
            }

            if (screenShaking == true)
            {
                screenShakeTimer.Start();
            }

            explosionTimer.Start();

            //staticTimer.Start();
            //haggarTimer.Start();
            //igniteTimer.Start();
            //fuseTimer.Start();

            roundTimer.Start();
            
            if (resumeAll)
            {
                winnerTextTimer.Start();
                decisionTimer.Start();
            }
        }

        List<Rect> newChallengerRects = new List<Rect>(1);

        Dictionary<string, ImageBrush> numbers = new Dictionary<string, ImageBrush>(20);

        List<Rect> plasmaRects = new List<Rect>(2);
        List<Rect> laserRects = new List<Rect>(0);

        List<ImageBrush> igniteImageBrushes = new List<ImageBrush>(2);

        List<Rect> lifeBarNameRects = new List<Rect>(3);

        private enum ExplosionSequenceState
        {
            exploding,
            settling,
            cracking,
            falling,
            destroyed,
            punching,
            ending,
            ended
        }

        ExplosionSequenceState explosionState = ExplosionSequenceState.exploding;

        private enum RtypeState
        {
            flying,
            warping,
            dead
        }

        RtypeState rtypeShipState = RtypeState.dead;

        private enum RampageState
        {
            climbing,
            punchingLeft,
            punchingDown,
            punchingRight,
            falling,
            burning,
            dead,
            spectating
        }

        RampageState georgeState = RampageState.dead;
        
        BitmapImage knifeImage;

        BitmapImage rampageBurning;
        
        private void CalculateScreenSaver()
        {
            if (screenSaverTimer.ElapsedMilliseconds <= screenSaverTimeInMinutes)
            {
                return;
            }
            else
            {
                PauseFrontend();
                screenSaver = true;

                ScreensaverStartStoryboard.Begin();
            }
        }
                
        
        
        private void RenderDecision()
        {
            switch (temporaryChampion)
            {
                case 0:
                    WinnerRectangle.Visibility = System.Windows.Visibility.Visible;
                    winnerTextTimer.Start();
                    showWinnerText = true;
                    break;
                case -1:
                    if (victoryTimer.ElapsedMilliseconds <= 3500)
                    {
                        // this is where you're going to animate the victory stuff or show the double knockout
                        return;
                    }
                    else
                    {
                        victoryTimer.Stop();
                        victoryTimer.Reset();

                        decidingWinner = false;
                        decisionMade = true;

                        RematchStartStoryboard.Begin();
                    }
                    break;
                case -2:
                    if (victoryTimer.ElapsedMilliseconds <= 3500)
                    {
                        // this is where you're going to animate the victory stuff or show the double knockout
                        return;
                    }
                    else
                    {
                        victoryTimer.Stop();
                        victoryTimer.Reset();

                        decidingWinner = false;
                        decisionMade = true;

                        RematchStartStoryboard.Begin();
                    }
                    break;
                case 1:
                    WinnerRectangle.Visibility = System.Windows.Visibility.Visible;
                    winnerTextTimer.Start();
                    showWinnerText = true;
                    break;
                default:
                    break;
            }
        }

        internal override void ProcessFrontendAction(string action, bool? inputDown)
        {
            // if we've launched a game, and the action isn't the exit key or one of the volume controls, we really don't even want to try processing anything
            if (itsGoTime == true && action != "BIG_BLUE_EXIT" && action != "RAMPAGE_EXIT" && action != "RTYPE_EXIT" && action != "BIG_BLUE_MUTE" && action != "BIG_BLUE_VOLUME_UP" && action != "BIG_BLUE_VOLUME_DOWN")
            {
                ReleaseInput(action);

                return;
            }

            switch (action)
            {
                case "BIG_BLUE_EXIT":
                case "RAMPAGE_EXIT":
                case "RTYPE_EXIT":
                    MenuAction(action, inputDown);
                    break;
                case "BIG_BLUE_MUTE":
                    if (inputDown == true)
                    {
                        BigBlue.NativeMethods.SendMessageW(windowHandle, BigBlue.NativeMethods.WM_APPCOMMAND, windowHandle, (IntPtr)BigBlue.NativeMethods.APPCOMMAND_VOLUME_MUTE);
                    }
                    break;
                case "BIG_BLUE_VOLUME_UP":
                    BigBlue.NativeMethods.SendMessageW(windowHandle, BigBlue.NativeMethods.WM_APPCOMMAND, windowHandle, (IntPtr)BigBlue.NativeMethods.APPCOMMAND_VOLUME_UP);
                    break;
                case "BIG_BLUE_VOLUME_DOWN":
                    BigBlue.NativeMethods.SendMessageW(windowHandle, BigBlue.NativeMethods.WM_APPCOMMAND, windowHandle, (IntPtr)BigBlue.NativeMethods.APPCOMMAND_VOLUME_DOWN);
                    break;
                case "RAMPAGE_FIRST_ITEM":
                case "RTYPE_FIRST_ITEM":
                    FirstItemKeyboardAction(action, inputDown);
                    break;
                case "RAMPAGE_LAST_ITEM":
                case "RTYPE_LAST_ITEM":
                    LastItemKeyboardAction(action, inputDown);
                    break;
                case "RAMPAGE_RANDOM_ITEM":
                case "RTYPE_RANDOM_ITEM":
                    RandomItemKeyboardAction(action, inputDown);
                    break;
                case "RAMPAGE_NEXT_ITEM":
                case "RTYPE_NEXT_ITEM":
                    if (inputDown == null)
                    {
                        // mouse version here
                        GameListMouseAction(action);
                    }
                    else
                    {
                        NextItemKeyboardAction(action, inputDown);
                    }
                    break;
                case "RAMPAGE_PREVIOUS_ITEM":
                case "RTYPE_PREVIOUS_ITEM":
                    if (inputDown == null)
                    {
                        // mouse version here
                        GameListMouseAction(action);
                    }
                    else
                    {
                        PreviousItemKeyboardAction(action, inputDown);
                    }
                    break;
                case "RAMPAGE_NEXT_PAGE":
                case "RTYPE_NEXT_PAGE":
                    if (inputDown == null)
                    {
                        // mouse version here
                        GameListMouseAction(action);
                    }
                    else
                    {
                        NextPageKeyboardAction(action, inputDown);
                    }
                    break;
                case "RAMPAGE_PREVIOUS_PAGE":
                case "RTYPE_PREVIOUS_PAGE":
                    if (inputDown == null)
                    {
                        // mouse version here
                        GameListMouseAction(action);
                    }
                    else
                    {
                        PreviousPageKeyboardAction(action, inputDown);
                    }
                    break;
                case "RAMPAGE_BACK":
                case "RTYPE_BACK":
                    //KeyTest.Text = "";
                    //foreach (string s in subFolderTrail)
                    //{
                    //  KeyTest.Text = KeyTest.Text + "\n" + s;
                    //}

                    GameListBackAction(action, inputDown);

                    break;
                case "RAMPAGE_PUNCH_LEFT":
                    RampagePunchLeftAction(action, inputDown);
                    break;
                case "RAMPAGE_PUNCH_RIGHT":
                    RampagePunchRightAction(action, inputDown);
                    break;
                case "RAMPAGE_SPECTATE":
                    RampageSpectateAction(action, inputDown);
                    break;
                case "RAMPAGE_START":
                case "RTYPE_START":
                    PlayerStartAction(action, inputDown);
                    break;
                case "RTYPE_SHOOT":
                    RtypeShootAction(action, inputDown);
                    break;
                case "RTYPE_WARP":
                    RtypeWarpAction(action, inputDown);
                    break;
                case "BIG_BLUE_SPEECH":
                    InitiateSpeechAction(action, inputDown);
                    break;
                case "RESTART":
                    PlayerMenuShortcutAction(action, BigBlue.Models.FrontEndExitMode.restart, inputDown);
                    break;
                case "SHUTDOWN":
                    PlayerMenuShortcutAction(action, BigBlue.Models.FrontEndExitMode.shutdown, inputDown);
                    break;
                case "QUIT_TO_DESKTOP":
                    PlayerMenuShortcutAction(action, BigBlue.Models.FrontEndExitMode.quit, inputDown);
                    break;
            }
        }

        private void MakeFightDecision()
        {
            if (decisionTimer.ElapsedMilliseconds <= 100)
            {
                return;
            }
            else
            {
                BigBlue.XAudio2Player.PauseSound("warp");
             
                decisionTimer.Stop();
                decisionTimer.Reset();

                if ((rtypeDestroyed == true || rtypeShipState == RtypeState.dead) && (georgeState == RampageState.burning || georgeState == RampageState.falling || georgeState == RampageState.dead))
                {
                    temporaryChampion = -1;
                }
                else
                {
                    if (rtypeDestroyed == true || rtypeShipState == RtypeState.dead)
                    {
                        temporaryChampion = 0;
                        champion = 0;
                    }
                    else if (georgeState == RampageState.dead || georgeState == RampageState.falling || georgeState == RampageState.burning)
                    {
                        temporaryChampion = 1;
                        champion = 1;
                    }
                    else
                    {
                        temporaryChampion = -2;
                    }
                }

                waitingForDeathAnimations = true;
            }
        }
        
        private void WaitForDeathAnimations()
        {
            switch (temporaryChampion)
            {
                case 0:
                    if (rtypeShipState == RtypeState.dead && rtypeDestroyed == true)
                    {
                        ProvisionDecision();
                    }

                    break;
                case -1:
                    if (georgeState == RampageState.dead && screenShaking == false && rtypeShipState == RtypeState.dead && rtypeDestroyed == true)
                    {
                        // show the DOUBLE KO graphics if they're both dead
                        if (DoubleKODecision.Visibility == System.Windows.Visibility.Hidden)
                        {
                            DoubleKODecision.Visibility = System.Windows.Visibility.Visible;
                        }

                        victoryTimer.Start();

                        ProvisionDecision();
                    }
                    break;
                case -2:
                    if (georgeState != RampageState.dead && rtypeShipState != RtypeState.dead && rtypeDestroyed == false && georgeState != RampageState.falling && georgeState != RampageState.burning)
                    {
                        // we're going to set the countdown numbers to the standard color for "00" a la Street Fighter 2
                        FirstDigit.Fill = numbers["00"];
                        SecondDigit.Fill = numbers["00"];

                        // show the DRAW GAME graphics if they're both alive
                        if (DrawGameRectangle.Visibility == System.Windows.Visibility.Hidden)
                        {
                            DrawGameRectangle.Visibility = System.Windows.Visibility.Visible;
                        }

                        victoryTimer.Start();

                        ProvisionDecision();
                    }
                    break;
                case 1:
                    if (georgeState == RampageState.dead && screenShaking == false)
                    {
                        ProvisionDecision();
                    }
                    break;
                default:
                    break;
            }
        }

        private void ProvisionDecision()
        {
            if (music == true)
            {
                BigBlue.XAudio2Player.StopSound("guile");
                BigBlue.XAudio2Player.StopSound("guilefast");
            }

            PauseFrontend();

            waitingForDeathAnimations = false;

            if (temporaryChampion == 0)
            {
                switch (rampageCharacterChoice)
                {
                    case 0:
                        winnerTextFrameToDisplay = 0;
                        break;
                    case 1:
                        winnerTextFrameToDisplay = 2;
                        break;
                    case 2:
                        winnerTextFrameToDisplay = 4;
                        break;
                }

                PlayVictoryDitty();
            }

            if (temporaryChampion == 1)
            {
                winnerTextFrameToDisplay = 6;
                PlayVictoryDitty();
            }

            celebratingVictory = true;

            RenderDecision();
        }

        private void TimeOverDecision()
        {
            // if both champion and challenger are still alive, kill off the challenger
            double georgeLife = One.Width;
            double rtypeLife = Two.Width;

            if (rtypeLife == georgeLife)
            {
                // this needs to be similar to the double KO only it needs to show the time out thing
                decidingWinner = true;
                // start the decision timer
                decisionTimer.Start();
            }
            else
            {
                if (georgeLife > rtypeLife)
                {
                    DestroyRtype();
                }
                else
                {
                    DestroyGeorge("suicide");
                }
            }

            FirstDigit.Fill = numbers["00"];
            SecondDigit.Fill = numbers["00"];
        }

        private void RoundCountdown()
        {
            if (roundTimer.ElapsedMilliseconds >= 800)
            {
                roundDuration = roundDuration - 1;

                string countDownString = roundDuration.ToString();

                if (roundDuration >= 10)
                {
                    currentDigitOne = countDownString.Substring(0, 1);
                    currentDigitTwo = countDownString.Substring(1, 1);
                }
                else
                {
                    currentDigitOne = "0";
                    currentDigitTwo = countDownString;
                }

                // we're only going to bother to switch the graphics when they're not already being animated by the low-on-time animation
                if (roundDuration >= 15)
                {
                    FirstDigit.Fill = numbers[currentDigitOne + numberType];
                    SecondDigit.Fill = numbers[currentDigitTwo + numberType];
                }

                roundTimer.Reset();

                if (roundDuration == -1)
                {
                    roundDuration = 99;
                    // hide countdown UI element here possibly
                    TimeOverDecision();
                }
                else
                {
                    roundTimer.Start();
                }
            }
        }

        private void StyleGameListTextBlocks()
        {
            for (int i = 0; i < numberOfTextBlockListItems; i++)
            {
                TextBlock gameListEntry = gameListElements[i];

                if (i == textBlockListHalfWayPoint)
                {
                    if (timeCode == 2)
                    {
                        gameListEntry.Background = null;
                        //gameListEntry.Effect = dropShadowEffect;
                        gameListEntry.Foreground = yellowBrush;
                    }
                    else
                    {
                        gameListEntry.Background = transparentBlueBrush;

                        if (timeCode == 3)
                        {
                            gameListEntry.Foreground = whiteBrush;
                        }
                        else
                        {
                            gameListEntry.Foreground = yellowBrush;
                        }
                        //gameListEntry.Effect = null;
                    }
                }
                else
                {
                    switch (timeCode)
                    {
                        case 0:
                            gameListEntry.Foreground = blueBrush;

                            //gameListEntry.Effect = null;
                            break;
                        case 1:
                        case 2:
                        case 3:
                            gameListEntry.Foreground = whiteBrush;
                            //gameListEntry.Effect = dropShadowEffect;
                            break;
                        default:
                            break;
                    }
                }
            }

            switch (timeCode)
            {
                case 0:
                case 1:
                    GameListOutlines.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                case 3:
                    GameListOutlines.Visibility = Visibility.Visible;
                    break;
            }
        }

        //TextOptions.SetTextRenderingMode(what, TextRenderingMode.ClearType);
        
        internal override void ReadConfigFile()
        {
            base.ReadConfigFile();
            
            videoMe.Volume = minimumVolume;

            bool.TryParse(ConfigNode["clock"]?.InnerText, out showClock);

            int staticTimeIndex = -1;

            int.TryParse(ConfigNode["timeofday"]?.InnerText, out staticTimeIndex);

            if (staticTimeIndex != -1)
            {
                dynamicTimeOfDay = false;
                timeCode = staticTimeIndex;
            }

            bool.TryParse(ConfigNode["snapshotflicker"]?.InnerText, out snapshotFlicker);

            bool.TryParse(ConfigNode["freeforall"]?.InnerText, out freeForAll);

            switch (ConfigNode["sf2art"]?.InnerText)
            {
                case "ww":
                    streetFighterEdition = 0;
                    streetFighterEditionAnimation = 0;
                    break;
                case "ce":
                    streetFighterEdition = 1;
                    streetFighterEditionAnimation = 0;
                    break;
                case "hf":
                    streetFighterEdition = 2;
                    streetFighterEditionAnimation = 1;
                    break;
                default:
                    streetFighterEdition = 1;
                    streetFighterEditionAnimation = 1;
                    break;
            }
        }
        
        private void PauseFrontend()
        {
            BigBlue.XAudio2Player.PauseAllSounds();
            
            PauseVideo(VideoElement, Video);

            timeCodeTimer.Stop();
            pausedBySystem = true;
            GameList.Visibility = System.Windows.Visibility.Collapsed;
            PressStartRectangle.Visibility = System.Windows.Visibility.Collapsed;

            PauseStopWatches(false);
            
            if (freeForAll == false)
            {
                if (DependencyPropertyHelper.GetValueSource(FrontEndContainer, Canvas.OpacityProperty).IsAnimated)
                {
                    RematchEndStoryboard.Pause();
                    RematchStartStoryboard.Pause();
                    ShowFrontEndStoryboard.Pause();
                }

                if (DependencyPropertyHelper.GetValueSource(VsOverlay, Canvas.OpacityProperty).IsAnimated)
                {
                    ShowVersusStoryboard.Pause();
                    HideVersusStoryboard.Pause();
                }
            }

            if (DependencyPropertyHelper.GetValueSource(RampageMonsterRectangle, Canvas.TopProperty).IsAnimated)
            {
                GeorgeClimbingStoryBoard.Pause();
                GeorgeFallingStoryBoard.Pause();
            }

            if (DependencyPropertyHelper.GetValueSource(Rtype, Canvas.LeftProperty).IsAnimated)
            {
                RtypeFlyingStoryboard.Pause();
            }

            if (DependencyPropertyHelper.GetValueSource(SkyGradient1, Canvas.LeftProperty).IsAnimated)
            {
                SkyStoryBoard.Pause();
            }

            if (DependencyPropertyHelper.GetValueSource(SkyGradient2, Canvas.LeftProperty).IsAnimated)
            {
                Sky2StoryBoard.Pause();
            }

            if (DependencyPropertyHelper.GetValueSource(SkyGradient3, Canvas.LeftProperty).IsAnimated)
            {
                Sky3StoryBoard.Pause();
            }

            if (DependencyPropertyHelper.GetValueSource(SkyGradient4, Canvas.LeftProperty).IsAnimated)
            {
                Sky4StoryBoard.Pause();
            }

            if (DependencyPropertyHelper.GetValueSource(ScreenDoor, Rectangle.OpacityProperty).IsAnimated)
            {
                ScreenStoryBoard.Pause();
            }

            if (DependencyPropertyHelper.GetValueSource(KnifeRectangle, Canvas.TopProperty).IsAnimated)
            {
                KnifeStoryboard.Pause();
            }

            if (DependencyPropertyHelper.GetValueSource(StaticRectangle, Rectangle.VisibilityProperty).IsAnimated)
            {
                shutdownStoryboard.Pause();
                regularlyScheduledProgrammingStoryboard.Pause();
            }

            if (DependencyPropertyHelper.GetValueSource(Blood.RenderTransform, MatrixTransform.MatrixProperty).IsAnimated)
            {
                blood1Storyboard.Pause(this);
            }

            if (DependencyPropertyHelper.GetValueSource(Blood2.RenderTransform, MatrixTransform.MatrixProperty).IsAnimated)
            {
                blood2Storyboard.Pause(this);
            }
            if (DependencyPropertyHelper.GetValueSource(Blood3.RenderTransform, MatrixTransform.MatrixProperty).IsAnimated)
            {

                blood3Storyboard.Pause(this);
            }

            if (DependencyPropertyHelper.GetValueSource(Blood4.RenderTransform, MatrixTransform.MatrixProperty).IsAnimated)
            {
                blood4Storyboard.Pause(this);
            }

            if (DependencyPropertyHelper.GetValueSource(RampageMonsterFlameRectangle.RenderTransform, MatrixTransform.MatrixProperty).IsAnimated)
            {
                if (flamingStoryboard != null)
                {
                    flamingStoryboard.Pause(this);
                }
            }

            

            if (timeCode == 0)
            {
                if (DependencyPropertyHelper.GetValueSource(Cloud1Rectangle, Canvas.LeftProperty).IsAnimated)
                {
                    Cloud1StoryBoard.Pause();
                }

                if (DependencyPropertyHelper.GetValueSource(Cloud2Rectangle, Canvas.LeftProperty).IsAnimated)
                {
                    Cloud2StoryBoard.Pause();
                }
            }
            else if (timeCode == 1)
            {
                if (DependencyPropertyHelper.GetValueSource(Cloud3Rectangle, Canvas.LeftProperty).IsAnimated)
                {
                    Cloud3StoryBoard.Pause();
                }
            }
            else if (timeCode == 2)
            {
                if (DependencyPropertyHelper.GetValueSource(Cloud4Rectangle, Canvas.LeftProperty).IsAnimated)
                {
                    Cloud4StoryBoard.Pause();
                }
            }
            else
            {
                if (DependencyPropertyHelper.GetValueSource(Cloud5Rectangle, Canvas.LeftProperty).IsAnimated)
                {
                    Cloud5StoryBoard.Pause();
                }
            }

            foreach (KeyValuePair<Guid, Storyboard> laserStoryboard in laserStoryboards)
            {
                laserStoryboard.Value.Pause();
            }

            for (int i = 0; i < buildingBlocks.Count(); i++)
            {
                if (DependencyPropertyHelper.GetValueSource(buildingDebris[i], Canvas.TopProperty).IsAnimated)
                {
                    debrisStoryboards[i].Pause();
                }
            }
            
            PauseTimeTransition();
        }

        private void ResumeVideo()
        {
            // if (categorySelection == false && shutdownSequenceActivated == false && videoUrls != null)
            if (roundInProgress == false && challengerIntroRunning == false && shutdownSequenceActivated == false)
            {
                if (videoFadeInStopwatch.ElapsedMilliseconds > 0)
                {
                    videoFadeInStopwatch.Start();
                }
                
                if (VideoElement.Source != null)
                {
                    if (videoFadeOutStopwatch.ElapsedMilliseconds > 0)
                    {
                        videoFadeOutStopwatch.Start();
                    }

                    ClockState fadeInState = videoFadeInStoryboard.GetCurrentState();

                    switch (fadeInState)
                    {
                        case ClockState.Active:
                            break;
                        case ClockState.Filling:
                            break;
                        case ClockState.Stopped:
                            if (isVideoPlaying == true)
                            {
                                VideoElement.Play();
                                PlayMediaElement();
                            }
                            break;
                    }
                    
                    if (DependencyPropertyHelper.GetValueSource(Video, Canvas.OpacityProperty).IsAnimated)
                    {
                        if (videoMaterializing == true)
                        {
                            if (videoFadeInStoryboard.GetIsPaused() == true)
                            {
                                videoFadeInStoryboard.Resume();
                            }
                        }

                        if (videoFading == true)
                        {
                            if (videoFadeOutStoryboard.GetIsPaused() == true)
                            {
                                videoFadeOutStoryboard.Resume();
                            }
                            
                        }
                    }

                    if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Video != null)
                    {
                        VideoElement.Play();   
                    }
                }
            }
        }

        private void ResumeFrontend()
        {
            BigBlue.XAudio2Player.ResumeAllSounds();
            
            if (shutdownSequenceActivated == false && roundInProgress == false)
            {
                ResumeVideo();
            }

            timeCodeTimer.Start();
            pausedBySystem = false;

            ResumeStopWatches(false);

            if (DependencyPropertyHelper.GetValueSource(FrontEndContainer, Canvas.OpacityProperty).IsAnimated)
            {
                RematchEndStoryboard.Resume();
                RematchStartStoryboard.Resume();
                ShowFrontEndStoryboard.Resume();
            }

            if (DependencyPropertyHelper.GetValueSource(VsOverlay, Canvas.OpacityProperty).IsAnimated)
            {
                ShowVersusStoryboard.Resume();
                HideVersusStoryboard.Resume();
            }

            if (DependencyPropertyHelper.GetValueSource(RampageMonsterRectangle, Canvas.TopProperty).IsAnimated)
            {
                GeorgeClimbingStoryBoard.Resume();
                GeorgeFallingStoryBoard.Resume();
            }

            if (rtypeDestroyed == false)
            {
                if (DependencyPropertyHelper.GetValueSource(Rtype, Canvas.LeftProperty).IsAnimated)
                {
                    RtypeFlyingStoryboard.Resume();
                }
            }

            if (DependencyPropertyHelper.GetValueSource(SkyGradient1, Canvas.LeftProperty).IsAnimated)
            {
                SkyStoryBoard.Resume();
            }

            if (DependencyPropertyHelper.GetValueSource(SkyGradient2, Canvas.LeftProperty).IsAnimated)
            {
                Sky2StoryBoard.Resume();
            }

            if (DependencyPropertyHelper.GetValueSource(SkyGradient3, Canvas.LeftProperty).IsAnimated)
            {
                Sky3StoryBoard.Resume();
            }

            if (DependencyPropertyHelper.GetValueSource(SkyGradient4, Canvas.LeftProperty).IsAnimated)
            {
                Sky4StoryBoard.Resume();
            }

            if (DependencyPropertyHelper.GetValueSource(ScreenDoor, Rectangle.OpacityProperty).IsAnimated)
            {
                ScreenStoryBoard.Resume();
            }

            if (DependencyPropertyHelper.GetValueSource(KnifeRectangle, Canvas.TopProperty).IsAnimated)
            {
                KnifeStoryboard.Resume();
            }

            if (DependencyPropertyHelper.GetValueSource(Blood.RenderTransform, MatrixTransform.MatrixProperty).IsAnimated)
            {
                blood1Storyboard.Resume(this);
            }

            if (DependencyPropertyHelper.GetValueSource(Blood2.RenderTransform, MatrixTransform.MatrixProperty).IsAnimated)
            {
                blood2Storyboard.Resume(this);
            }

            if (DependencyPropertyHelper.GetValueSource(Blood3.RenderTransform, MatrixTransform.MatrixProperty).IsAnimated)
            {
                blood3Storyboard.Resume(this);
            }

            if (DependencyPropertyHelper.GetValueSource(Blood4.RenderTransform, MatrixTransform.MatrixProperty).IsAnimated)
            {
                blood4Storyboard.Resume(this);
            }

            if (DependencyPropertyHelper.GetValueSource(RampageMonsterFlameRectangle.RenderTransform, MatrixTransform.MatrixProperty).IsAnimated)
            {
                if (flamingStoryboard != null)
                {
                    flamingStoryboard.Resume(this);
                }
            }

            if (DependencyPropertyHelper.GetValueSource(StaticRectangle, Rectangle.VisibilityProperty).IsAnimated)
            {
                // this needs a check to see whether it just came out of the "show winner" sequence
                if (challengerIntroRunning == false)
                {
                    shutdownStoryboard.Resume();
                }

                regularlyScheduledProgrammingStoryboard.Resume();
            }
            
            if (timeCode == 0)
            {
                if (DependencyPropertyHelper.GetValueSource(Cloud1Rectangle, Canvas.LeftProperty).IsAnimated)
                {
                    Cloud1StoryBoard.Resume();
                }

                if (DependencyPropertyHelper.GetValueSource(Cloud2Rectangle, Canvas.LeftProperty).IsAnimated)
                {
                    Cloud2StoryBoard.Resume();
                }
            }
            else if (timeCode == 1)
            {
                if (DependencyPropertyHelper.GetValueSource(Cloud3Rectangle, Canvas.LeftProperty).IsAnimated)
                {
                    Cloud3StoryBoard.Resume();
                }
            }
            else if (timeCode == 2)
            {
                if (DependencyPropertyHelper.GetValueSource(Cloud4Rectangle, Canvas.LeftProperty).IsAnimated)
                {
                    Cloud4StoryBoard.Resume();
                }
            }
            else
            {
                if (DependencyPropertyHelper.GetValueSource(Cloud5Rectangle, Canvas.LeftProperty).IsAnimated)
                {
                    Cloud5StoryBoard.Resume();
                }
            }

            for (int i = 0; i < buildingBlocks.Count(); i++)
            {
                if (DependencyPropertyHelper.GetValueSource(buildingDebris[i], Canvas.TopProperty).IsAnimated)
                {
                    debrisStoryboards[i].Resume();
                }
            }

            foreach (KeyValuePair<Guid, Storyboard> laserStoryboard in laserStoryboards)
            {
                laserStoryboard.Value.Resume();
            }

            if (celebratingVictory == false && shutdownSequenceActivated == false && roundInProgress == false && challengerIntroRunning == false && temporaryChampion != -1 && temporaryChampion != -2)
            {
                GameList.Visibility = System.Windows.Visibility.Visible;

                //TODO
                /*
                if (freeForAll == false)
                {
                    PressStartRectangle.Visibility = System.Windows.Visibility.Visible;
                }
                */
            }
            
            UpdateClock();
            
            ResumeTimeTransition();
        }

        private void ResumeTimeTransition()
        {
            switch (timeCode)
            {
                case 0:
                    SkyGradientMorningStoryBoard1.Resume();
                    SkyGradientMorningStoryBoard2.Resume();
                    SkyGradientMorningStoryBoard3.Resume();
                    SkyGradientMorningStoryBoard4.Resume();
                    SkyGradientMorningStoryBoard5.Resume();

                    Stippled1MorningStoryBoard.Resume();
                    Stippled2MorningStoryBoard.Resume();
                    Stippled3MorningStoryBoard.Resume();
                    Stippled4MorningStoryBoard.Resume();
                    break;
                case 1:
                    SkyGradientSunsetStoryBoard1.Resume();
                    SkyGradientSunsetStoryBoard2.Resume();
                    SkyGradientSunsetStoryBoard3.Resume();
                    SkyGradientSunsetStoryBoard4.Resume();
                    SkyGradientSunsetStoryBoard5.Resume();

                    Stippled1SunsetStoryBoard.Resume();
                    Stippled2SunsetStoryBoard.Resume();
                    Stippled3SunsetStoryBoard.Resume();
                    Stippled4SunsetStoryBoard.Resume();
                    break;
                case 2:
                    SkyGradientNightStoryBoard1.Resume();
                    SkyGradientNightStoryBoard2.Resume();
                    SkyGradientNightStoryBoard3.Resume();
                    SkyGradientNightStoryBoard4.Resume();
                    SkyGradientNightStoryBoard5.Resume();

                    Stippled1NightStoryBoard.Resume();
                    Stippled2NightStoryBoard.Resume();
                    Stippled3NightStoryBoard.Resume();
                    Stippled4NightStoryBoard.Resume();
                    break;
                case 3:
                    SkyGradientDawnStoryBoard1.Resume();
                    SkyGradientDawnStoryBoard2.Resume();
                    SkyGradientDawnStoryBoard3.Resume();
                    SkyGradientDawnStoryBoard4.Resume();
                    SkyGradientDawnStoryBoard5.Resume();

                    Stippled1DawnStoryBoard.Resume();
                    Stippled2DawnStoryBoard.Resume();
                    Stippled3DawnStoryBoard.Resume();
                    Stippled4DawnStoryBoard.Resume();
                    break;
                default:
                    break;
            }
        }

        private void PauseTimeTransition()
        {
            switch (timeCode)
            {
                case 0:
                    SkyGradientMorningStoryBoard1.Pause();
                    SkyGradientMorningStoryBoard2.Pause();
                    SkyGradientMorningStoryBoard3.Pause();
                    SkyGradientMorningStoryBoard4.Pause();
                    SkyGradientMorningStoryBoard5.Pause();

                    Stippled1MorningStoryBoard.Pause();
                    Stippled2MorningStoryBoard.Pause();
                    Stippled3MorningStoryBoard.Pause();
                    Stippled4MorningStoryBoard.Pause();
                    break;
                case 1:
                    SkyGradientSunsetStoryBoard1.Pause();
                    SkyGradientSunsetStoryBoard2.Pause();
                    SkyGradientSunsetStoryBoard3.Pause();
                    SkyGradientSunsetStoryBoard4.Pause();
                    SkyGradientSunsetStoryBoard5.Pause();

                    Stippled1SunsetStoryBoard.Pause();
                    Stippled2SunsetStoryBoard.Pause();
                    Stippled3SunsetStoryBoard.Pause();
                    Stippled4SunsetStoryBoard.Pause();
                    break;
                case 2:
                    SkyGradientNightStoryBoard1.Pause();
                    SkyGradientNightStoryBoard2.Pause();
                    SkyGradientNightStoryBoard3.Pause();
                    SkyGradientNightStoryBoard4.Pause();
                    SkyGradientNightStoryBoard5.Pause();

                    Stippled1NightStoryBoard.Pause();
                    Stippled2NightStoryBoard.Pause();
                    Stippled3NightStoryBoard.Pause();
                    Stippled4NightStoryBoard.Pause();
                    break;
                case 3:
                    SkyGradientDawnStoryBoard1.Pause();
                    SkyGradientDawnStoryBoard2.Pause();
                    SkyGradientDawnStoryBoard3.Pause();
                    SkyGradientDawnStoryBoard4.Pause();
                    SkyGradientDawnStoryBoard5.Pause();

                    Stippled1DawnStoryBoard.Pause();
                    Stippled2DawnStoryBoard.Pause();
                    Stippled3DawnStoryBoard.Pause();
                    Stippled4DawnStoryBoard.Pause();
                    break;
                default:
                    break;
            }
        }

        private void CleanUpLaserBeams()
        {
            foreach (KeyValuePair<Guid, Rectangle> laser in laserBeams)
            {
                FrontEndContainer.Children.Remove(laserBeams[laser.Key]);
            }

            laserStoryboards.Clear();
            laserBeams.Clear();
        }
        
        private void ToggleMenu()
        {
            selectedMenuIndex = 0;

            if (MainMenu.Visibility == System.Windows.Visibility.Collapsed)
            {
                menuOpen = true;
                
                RenderMainMenu(MainMenu, false, yellowBrush, whiteBrush);

                MainMenu.Visibility = System.Windows.Visibility.Visible;

                PauseFrontend();

                if (showClock)
                {
                    ClockCanvas.Visibility = Visibility.Collapsed;
                }

                BigBlue.XAudio2Player.PlaySound(ExitListSoundKey, null);
            }
            else
            {
                menuOpen = false;

                MainMenu.Visibility = System.Windows.Visibility.Collapsed;

                if (celebratingVictory == true)
                {
                    BigBlue.XAudio2Player.ResumeSound("georgevictory");
                    BigBlue.XAudio2Player.ResumeSound("rtypevictory");
                    
                    if (temporaryChampion == -1 || temporaryChampion == -2)
                    {
                        victoryTimer.Start();
                    }

                    RematchEndStoryboard.Resume();
                    RematchStartStoryboard.Resume();
                }
                else if (challengerJoined == true || challengerIntroRunning == true)
                {
                    // when you toggle the menu while a challenger is joining a game, you don't want to resume everything, or characters will animate and so on
                    RematchEndStoryboard.Resume();
                    RematchStartStoryboard.Resume();
                    ShowVersusStoryboard.Resume();
                    HideVersusStoryboard.Resume();
                    ShowFrontEndStoryboard.Resume();

                    if (challengerJoined == true)
                    {
                        newChallengerTimer.Start();
                    }

                    pausedBySystem = false;

                    BigBlue.XAudio2Player.ResumeSound("newchallenger");
                    BigBlue.XAudio2Player.ResumeSound("versus");
                    BigBlue.XAudio2Player.ResumeSound("fight");
                }
                else
                {
                    // it should only resume the sounds if it's not paused by the system
                    //BigBlue.XAudio2Player.ResumeAllSounds();

                    ResumeFrontend();
                }
                
                UpdateClock();

                BigBlue.XAudio2Player.PlaySound(SelectListSoundKey, null);
            }
        }
        
        private void StartShutdown(bool closeMenu)
        {
            if (!closeMenu)
            {
                PauseVideo(VideoElement, Video);
                GameList.Visibility = System.Windows.Visibility.Collapsed;
                PressStartRectangle.Visibility = System.Windows.Visibility.Collapsed;
            }

            ReleaseAllInputs();
            RtypeDropWarp();

            shutdownStoryboard.Stop();
            regularlyScheduledProgrammingStoryboard.Stop();
            KnifeStoryboard.Stop();
            haggarInDanger = false;
            BigBlue.XAudio2Player.StopSound("fuse");
            canThrowKnife = false;
            Canvas.SetTop(KnifeRectangle, -KnifeRectangle.Height);
            HaggarCanvas.Visibility = System.Windows.Visibility.Visible;
            haggarFrameToDisplay = 0;
            haggerSpeed = 0;
            fuseFrameToDisplay = 0;
            igniteFrameToDisplay = 0;
            IgniteRectangle.Opacity = 1;

            shutdownSequenceActivated = true;

            if (marqueeDisplay == true)
            {
                marqueeWindow.damndAnimationTimer.Tick += marqueeWindow.animateSecondaryDisplay;
                marqueeWindow.damndAnimationTimer.Start();
            }

            if (flyerDisplay == true)
            {
                flyerWindow.damndAnimationTimer.Tick += flyerWindow.animateSecondaryDisplay;
                flyerWindow.damndAnimationTimer.Start();
            }

            if (instructionDisplay == true)
            {
                instructionWindow.damndAnimationTimer.Tick += instructionWindow.animateSecondaryDisplay;
                instructionWindow.damndAnimationTimer.Start();
            }

            if (closeMenu)
            {
                ToggleMenu();
            }
            
            shutdownStoryboard.Begin();

            if (challengerIntroRunning == true || showWinnerText == true || decidingWinner == true || waitingForDeathAnimations == true || celebratingVictory == true || (freeForAll == false && (georgeState == RampageState.falling || georgeState == RampageState.burning)))
            {
                shutdownStoryboard.Pause();
            }
        }
        
        
                
        private void RespawnBuilding()
        {
            int debrisIndex = 0;

            foreach (Rectangle buildingBlock in buildingBlocks)
            {
                buildingBlock.SetValue(FrameworkElement.TagProperty, 0);
                double chunkTop = chunkHeight * debrisIndex;
                Canvas.SetTop(buildingBlock, chunkTop);

                buildingBlock.Fill = buildingBlockAnimationFrames[timeCode][0];

                Rectangle debris = buildingDebris[debrisIndex];
                debris.Fill = buildingDebrisAnimationFrames[timeCode][0];

                buildingBlockDecals[debrisIndex].Fill = null;
                buildingBlockDecals[debrisIndex].SetValue(FrameworkElement.TagProperty, 0);

                debris.SetValue(FrameworkElement.TagProperty, 0);
                debris.Visibility = System.Windows.Visibility.Hidden;

                // + (26 * resolutionXMultiplier))
                Canvas.SetTop(debris, chunkTop);

                debrisIndex = debrisIndex + 1;
            }
        }

        Storyboard blood1Storyboard;
        Storyboard blood2Storyboard;
        Storyboard blood3Storyboard;
        Storyboard blood4Storyboard;

        private void ProvisionFighterBlood()
        {
            double bloodSpeed = 0.4;

            for (int bloodIndex = 0; bloodIndex < 4; bloodIndex++)
            {
                ImageBrush bloodBrush = new ImageBrush();

                bloodBrush.ImageSource = BigBlue.ImageLoading.loadImage("fighterblood" + bloodIndex.ToString() + ".png", integerMultiplier);
                bloodBrush.AlignmentX = AlignmentX.Left;
                bloodBrush.AlignmentY = AlignmentY.Top;
                bloodBrush.TileMode = TileMode.None;
                RenderOptions.SetBitmapScalingMode(bloodBrush, BitmapScalingMode.NearestNeighbor);
                bloodBrush.Freeze();
                fighterBloodBrushes.Add(bloodBrush);
            }
            
            double bloodWidth = Math.Ceiling(19 * resolutionXMultiplier);
            double bloodHeight = Math.Ceiling(10 * resolutionXMultiplier);

            Blood.Width = bloodWidth;
            Blood.Height = bloodHeight;
            Blood2.Width = bloodWidth;
            Blood2.Height = bloodHeight;
            Blood3.Width = bloodWidth;
            Blood3.Height = bloodHeight;
            Blood4.Width = bloodWidth;
            Blood4.Height = bloodHeight;

            // starting point
            double bloodStartX = width - (FightersRectangle.Width + Canvas.GetRight(FightersRectangle) - (82 * resolutionXMultiplier));
            double bloodStartY = 392 * resolutionXMultiplier;
            double bloodContainerHeight = FightersRectangle.Height; // 694
            double bottomOfBloodContainer = ForegroundObjects.Height + (2 * resolutionXMultiplier); // 694 + 2 = 696

            //resolutionXMultiplier = 1;

            // Create a MatrixTransform. This transform 
            // will be used to move the button.
            MatrixTransform blood1MatrixTransform = new MatrixTransform();
            Blood.RenderTransform = blood1MatrixTransform;

            // Register the transform's name with the page 
            // so that it can be targeted by a Storyboard. 
            this.RegisterName("Blood1MatrixTransform", blood1MatrixTransform);


            // Create the animation path.
            PathGeometry blood1AnimationPath = new PathGeometry();
            PathFigure pFigure = new PathFigure();
            pFigure.StartPoint = new Point(bloodStartX, bloodStartY);
            PolyBezierSegment pBezierSegment = new PolyBezierSegment();
            pBezierSegment.Points.Add(new Point(bloodStartX - (16 * resolutionXMultiplier), bloodStartY - (22 * resolutionXMultiplier)));
            pBezierSegment.Points.Add(new Point(bloodStartX - (981 * resolutionXMultiplier), bloodStartY - (342 * resolutionXMultiplier)));
            pBezierSegment.Points.Add(new Point(bloodStartX - (1161 * resolutionXMultiplier), bottomOfBloodContainer));
            
            pFigure.Segments.Add(pBezierSegment);
            pFigure.Freeze();
            blood1AnimationPath.Figures.Add(pFigure);

            // Freeze the PathGeometry for performance benefits.
            blood1AnimationPath.Freeze();

            // Create a MatrixAnimationUsingPath to move the 
            // button along the path by animating 
            // its MatrixTransform.
            MatrixAnimationUsingPath blood1MatrixAnimation =
                new MatrixAnimationUsingPath();
            blood1MatrixAnimation.PathGeometry = blood1AnimationPath;
            blood1MatrixAnimation.Duration = TimeSpan.FromSeconds(bloodSpeed);
            blood1MatrixAnimation.DoesRotateWithTangent = true;
            //blood1MatrixAnimation.RepeatBehavior = RepeatBehavior.Forever;

            // Set the animation to target the Matrix property 
            // of the MatrixTransform named "ButtonMatrixTransform".
            Storyboard.SetTargetName(blood1MatrixAnimation, "Blood1MatrixTransform");
            Storyboard.SetTargetProperty(blood1MatrixAnimation,
                new PropertyPath(MatrixTransform.MatrixProperty));
            blood1MatrixAnimation.Freeze();
            // Create a Storyboard to contain and apply the animation.
            blood1Storyboard = new Storyboard();
            blood1Storyboard.Children.Add(blood1MatrixAnimation);
            blood1Storyboard.Freeze();
            
            // Create a MatrixTransform. This transform 
            // will be used to move the button.
            MatrixTransform blood2MatrixTransform = new MatrixTransform();
            Blood2.RenderTransform = blood2MatrixTransform;

            // Register the transform's name with the page 
            // so that it can be targeted by a Storyboard. 
            this.RegisterName("blood2MatrixTransform", blood2MatrixTransform);


            // Create the animation path.
            PathGeometry blood2AnimationPath = new PathGeometry();
            PathFigure pFigure2 = new PathFigure();
            pFigure2.StartPoint = new Point(bloodStartX, bloodStartY);
            PolyBezierSegment pBezierSegment2 = new PolyBezierSegment();
            pBezierSegment2.Points.Add(new Point(bloodStartX - (16 * resolutionXMultiplier), bloodStartY - (22 * resolutionXMultiplier)));
            pBezierSegment2.Points.Add(new Point(bloodStartX - (921 * resolutionXMultiplier), bloodStartY - (322 * resolutionXMultiplier)));
            pBezierSegment2.Points.Add(new Point(bloodStartX - (1086 * resolutionXMultiplier), bottomOfBloodContainer));

            pFigure2.Segments.Add(pBezierSegment2);
            pFigure2.Freeze();
            blood2AnimationPath.Figures.Add(pFigure2);

            // Freeze the PathGeometry for performance benefits.
            blood2AnimationPath.Freeze();

            // Create a MatrixAnimationUsingPath to move the 
            // button along the path by animating 
            // its MatrixTransform.
            MatrixAnimationUsingPath blood2MatrixAnimation =
                new MatrixAnimationUsingPath();
            blood2MatrixAnimation.PathGeometry = blood2AnimationPath;
            blood2MatrixAnimation.Duration = TimeSpan.FromSeconds(bloodSpeed);
            blood2MatrixAnimation.DoesRotateWithTangent = true;
            //matrixAnimation.RepeatBehavior = RepeatBehavior.Forever;

            // Set the animation to target the Matrix property 
            // of the MatrixTransform named "ButtonMatrixTransform".
            Storyboard.SetTargetName(blood2MatrixAnimation, "blood2MatrixTransform");
            Storyboard.SetTargetProperty(blood2MatrixAnimation,
                new PropertyPath(MatrixTransform.MatrixProperty));
            blood2MatrixAnimation.Freeze();
            // Create a Storyboard to contain and apply the animation.
            blood2Storyboard = new Storyboard();
            blood2Storyboard.Children.Add(blood2MatrixAnimation);
            blood2Storyboard.Freeze();

            // create a MatrixTransform to animate the third blood drop
            MatrixTransform blood3MatrixTransform = new MatrixTransform();

            Blood3.RenderTransform = blood3MatrixTransform;

            // Register the transform's name with the page so that it can be targeted by a Storyboard. 
            this.RegisterName("blood3MatrixTransform", blood3MatrixTransform);

            // Create the animation path.
            PathGeometry blood3AnimationPath = new PathGeometry();
            PathFigure pFigure3 = new PathFigure();
            pFigure3.StartPoint = new Point(bloodStartX, bloodStartY);
            PolyBezierSegment pBezierSegment3 = new PolyBezierSegment();
            pBezierSegment3.Points.Add(new Point(bloodStartX - (16 * resolutionXMultiplier), bloodStartY - (22 * resolutionXMultiplier)));
            pBezierSegment3.Points.Add(new Point(bloodStartX - (861 * resolutionXMultiplier), bloodStartY - (302 * resolutionXMultiplier)));
            pBezierSegment3.Points.Add(new Point(bloodStartX - (1011 * resolutionXMultiplier), bottomOfBloodContainer));


            pFigure3.Segments.Add(pBezierSegment3);
            pFigure3.Freeze();
            blood3AnimationPath.Figures.Add(pFigure3);

            // Freeze the PathGeometry for performance benefits.
            blood3AnimationPath.Freeze();

            // Create a MatrixAnimationUsingPath to move the 
            // button along the path by animating 
            // its MatrixTransform.
            MatrixAnimationUsingPath blood3MatrixAnimation =
                new MatrixAnimationUsingPath();
            blood3MatrixAnimation.PathGeometry = blood3AnimationPath;
            blood3MatrixAnimation.Duration = TimeSpan.FromSeconds(bloodSpeed);
            blood3MatrixAnimation.DoesRotateWithTangent = true;
            //matrixAnimation.RepeatBehavior = RepeatBehavior.Forever;

            // Set the animation to target the Matrix property 
            // of the MatrixTransform named "ButtonMatrixTransform".
            Storyboard.SetTargetName(blood3MatrixAnimation, "blood3MatrixTransform");
            Storyboard.SetTargetProperty(blood3MatrixAnimation,
                new PropertyPath(MatrixTransform.MatrixProperty));

            blood3MatrixAnimation.Freeze();
            // Create a Storyboard to contain and apply the animation.
            blood3Storyboard = new Storyboard();
            blood3Storyboard.Children.Add(blood3MatrixAnimation);
            blood3Storyboard.Freeze();

            // create a matrix transform to animate the blood
            MatrixTransform blood4MatrixTransform = new MatrixTransform();
            Blood4.RenderTransform = blood4MatrixTransform;

            // Register the transform's name with the page 
            // so that it can be targeted by a Storyboard. 
            this.RegisterName("blood4MatrixTransform", blood4MatrixTransform);


            // Create the animation path.
            PathGeometry blood4AnimationPath = new PathGeometry();
            PathFigure pFigure4 = new PathFigure();
            pFigure4.StartPoint = new Point(bloodStartX, bloodStartY);
            PolyBezierSegment pBezierSegment4 = new PolyBezierSegment();
            pBezierSegment4.Points.Add(new Point(bloodStartX - (16 * resolutionXMultiplier), bloodStartY - (22 * resolutionXMultiplier)));
            pBezierSegment4.Points.Add(new Point(bloodStartX - (801 * resolutionXMultiplier), bloodStartY - (282 * resolutionXMultiplier)));
            pBezierSegment4.Points.Add(new Point(bloodStartX - (936 * resolutionXMultiplier), bottomOfBloodContainer));

            pFigure4.Segments.Add(pBezierSegment4);
            pFigure4.Freeze();
            blood4AnimationPath.Figures.Add(pFigure4);

            // Freeze the PathGeometry for performance benefits.
            blood4AnimationPath.Freeze();

            // Create a MatrixAnimationUsingPath to move the 
            // button along the path by animating 
            // its MatrixTransform.
            MatrixAnimationUsingPath blood4MatrixAnimation =
                new MatrixAnimationUsingPath();
            blood4MatrixAnimation.PathGeometry = blood4AnimationPath;
            blood4MatrixAnimation.Duration = TimeSpan.FromSeconds(bloodSpeed);
            blood4MatrixAnimation.DoesRotateWithTangent = true;
            //matrixAnimation.RepeatBehavior = RepeatBehavior.Forever;

            // Set the animation to target the Matrix property 
            // of the MatrixTransform named "ButtonMatrixTransform".
            Storyboard.SetTargetName(blood4MatrixAnimation, "blood4MatrixTransform");
            Storyboard.SetTargetProperty(blood4MatrixAnimation,
                new PropertyPath(MatrixTransform.MatrixProperty));
            blood4MatrixAnimation.Freeze();
            // Create a Storyboard to contain and apply the animation.
            blood4Storyboard = new Storyboard();
            blood4Storyboard.Children.Add(blood4MatrixAnimation);
            blood4Storyboard.Freeze();
        }

        private void RtypeShootLaser()
        {
            PlayRtypeLaserSound();
            //ImageBrush rtypeImageBrush = (ImageBrush)Rtype.Fill;
            Rtype.Fill = rtypeAnimationFrames[timeCode][1];
            rtypeCurrentFrame = 1;
            //rtypeImageBrush.Viewbox = rtypeRects[1];
            //RtypeImage.Viewbox = rtypeRects[1];
            rtypeFrameToDisplay = 0;

            Rectangle laserBeamRect = new Rectangle();
            laserBeamRect.IsHitTestVisible = false;
            laserBeamRect.Width = rtypeLaserWidth;
            laserBeamRect.Height = rtypeLaserHeight;
            laserBeamRect.Fill = rtypeLaserBrush;

            Canvas.SetTop(laserBeamRect, rtypeLaserVerticalPosition);
            Canvas.SetZIndex(laserBeamRect, 60);

            Guid laserID = Guid.NewGuid();

            // add laser rectangle to the canvas
            FrontEndContainer.Children.Add(laserBeamRect);

            // add laser beam to the Rectangle list
            laserBeams.Add(laserID, laserBeamRect);

            Storyboard laserStoryboard = new Storyboard();
            //laserStoryboards.Add(laserID, laserStoryboard);

            double laserStart = currentRtypeLeftPosition + rtypeShipOnlyLength;

            DoubleAnimation laserAnimation = new DoubleAnimation();

            laserAnimation.From = laserStart;
            laserAnimation.To = width + laserStart + rtypeLaserWidth;
            laserAnimation.SpeedRatio = horizontalAnimationSpeedRatio;
            laserAnimation.Duration = new
Duration(TimeSpan.FromSeconds(1));
            laserAnimation.AutoReverse = false;

            Storyboard.SetTarget(laserAnimation,
laserBeamRect);

            // Set the attached properties of Canvas.Left and Canvas.Top
            // to be the target properties of the two respective DoubleAnimations.
            Storyboard.SetTargetProperty(laserAnimation, new
PropertyPath("(Canvas.Left)"));

            laserAnimation.Freeze();

            laserStoryboard.Children.Add(laserAnimation);


            laserStoryboard.Freeze();
            laserStoryboards.Add(laserID, laserStoryboard);

            laserStoryboard.Begin();
        }

        private void MakeCrowdFlee()
        {
            int crowdPriority = 110;

            foreach (Rectangle crowd in spectatorTiles)
            {
                Panel.SetZIndex(crowd, crowdPriority);

                crowdPriority = crowdPriority - 1;

                Storyboard crowdBounceStoryboard = new Storyboard();
                DoubleAnimation crowdBounceAnimation = new DoubleAnimation();

                crowdBounceAnimation.From = 0;
                crowdBounceAnimation.To = -(Math.Ceiling(16 * resolutionXMultiplier));
                crowdBounceAnimation.SpeedRatio = 6.0;
                crowdBounceAnimation.Duration = new
    Duration(TimeSpan.FromSeconds(1));
                crowdBounceAnimation.AutoReverse = true;
                crowdBounceAnimation.RepeatBehavior = RepeatBehavior.Forever;

                Storyboard.SetTarget(crowdBounceAnimation,
    crowd);
                // Set the attached properties of Canvas.Left and Canvas.Top
                // to be the target properties of the two respective DoubleAnimations.
                Storyboard.SetTargetProperty(crowdBounceAnimation, new
    PropertyPath("(Canvas.Bottom)"));

                crowdBounceAnimation.Freeze();

                crowdBounceStoryboard.Children.Add(crowdBounceAnimation);

                crowdBounceStoryboard.Freeze();

                crowdBounceStoryboard.Begin();

                Storyboard crowdStoryboard = new Storyboard();
                DoubleAnimation crowdAnimation = new DoubleAnimation();

                double crowdStart = Canvas.GetRight(crowd);

                crowdAnimation.From = crowdStart;

                if (crowdStart <= width / 2)
                {
                    crowdAnimation.To = -(Math.Ceiling(480 * resolutionXMultiplier) + width);
                }
                else
                {
                    crowdAnimation.To = (width * 2) - Math.Ceiling(480 * resolutionXMultiplier);
                }



                crowdAnimation.SpeedRatio = 1.0;
                crowdAnimation.Duration = new
    Duration(TimeSpan.FromSeconds(7));
                crowdAnimation.AutoReverse = false;

                Storyboard.SetTarget(crowdAnimation,
    crowd);

                // Set the attached properties of Canvas.Left and Canvas.Top
                // to be the target properties of the two respective DoubleAnimations.
                Storyboard.SetTargetProperty(crowdAnimation, new
    PropertyPath("(Canvas.Right)"));
                crowdAnimation.Freeze();

                crowdStoryboard.Children.Add(crowdAnimation);

                crowdStoryboard.Freeze();

                crowdStoryboard.Begin();
            }
        }

        Rect georgeCollisionRect1 = new System.Windows.Rect();
        Rect georgeCollisionRect2 = new Rect();
        Rect georgeCollisionRect3 = new Rect();

        Rect buildingCollisionBox = new Rect();
        
        private void CheckBuildingCollisions(Rect fistToCheck)
        {
            for (int i = 0; i < numberOfBuildingChunks; i++)
            {
                Rectangle item = buildingBlocks[i];

                int originalBlockCondition = (int)item.GetValue(FrameworkElement.TagProperty);
                
                buildingCollisionBox.X = Canvas.GetLeft(item);
                buildingCollisionBox.Y = Canvas.GetTop(item);
                
                if (fistToCheck.IntersectsWith(buildingCollisionBox) == true)
                {
                    if (originalBlockCondition == 1 && (fistToCheck.Y >= buildingCollisionBox.Y) && ((fistToCheck.Y + fistToCheck.Height) <= (buildingCollisionBox.Y + buildingCollisionBox.Height)))
                    {
                        PlayTruckedBuildingSound();
                        debrisElapsedMilliseconds[i] = 0;
                        //debrisTimers[i].Restart();

                        Rectangle itemDebris = buildingDebris[i];
                        itemDebris.SetValue(FrameworkElement.TagProperty, 1);
                        //itemDebris.Opacity = 1;
                        itemDebris.Visibility = System.Windows.Visibility.Visible;
                        //itemDebris.Fill = buildingDebrisAnimationFrames[timeCode][0];

                        // when we're ready to punch a hole in the building, we'll choose a random hole to set the block to

                        // set the random hole value
                        // these are all the possible indexes for holes in the sprite sheet
                        int rInt = r.Next(5, 15);

                        buildingBlockDecals[i].SetValue(FrameworkElement.TagProperty, 2);

                        item.SetValue(FrameworkElement.TagProperty, rInt);
                        buildingBlocks[i].Fill = buildingBlockAnimationFrames[timeCode][rInt];

                        //buildingBlockDecals[i].Fill = new SolidColorBrush(Colors.Purple);

                        // play the falling animation
                        debrisStoryboards[i].Begin();

                        return;
                    }
                    else
                    {
                        if (originalBlockCondition == 0)
                        {
                            buildingBlocks[i].Fill = buildingBlockAnimationFrames[timeCode][1];
                            item.SetValue(FrameworkElement.TagProperty, 1);
                        }
                    }
                }
            }

            PlayGeorgePunchSound();
        }

        private void GeorgePunchBuilding()
        {
            // we test against a different collision box depending on whether george is climbing up or down
            if (climbingUp == true)
            {
                //georgeCollisionRect2.X = Canvas.GetLeft(George2CollisionBox);
                //georgeCollisionRect2.Y = Canvas.GetTop(George2CollisionBox);
                
                CheckBuildingCollisions(georgeCollisionRect2);
            }
            else
            {
                //georgeCollisionRect3.X = Canvas.GetLeft(George3CollisionBox);
                //georgeCollisionRect3.Y = Canvas.GetTop(George3CollisionBox);
                
                CheckBuildingCollisions(georgeCollisionRect3);
            }
        }

        private const int BUILDING_DEBRIS_ANIMATION_SPEED = 300;

        private void AnimateDebris()
        {
            for (int i = 0; i < numberOfBuildingChunks; i++)
            {
                Rectangle debris = buildingDebris[i];
                debrisElapsedMilliseconds[i] = debrisElapsedMilliseconds[i] + elapsedMilliseconds;
                //Stopwatch debrisTimer = debrisTimers[i];

                //if (debrisTimer.ElapsedMilliseconds > BUILDING_DEBRIS_ANIMATION_SPEED)
                if (debrisElapsedMilliseconds[i] > BUILDING_DEBRIS_ANIMATION_SPEED)
                {
                    //debrisTimer.Reset();
                    //debrisTimer.Start();
                    debrisElapsedMilliseconds[i] = 0;

                    int debrisState = (int)debris.GetValue(FrameworkElement.TagProperty);

                    if (debrisState > 0)
                    {
                        debris.Fill = buildingDebrisAnimationFrames[timeCode][debrisState];

                        if (debrisState < 3)
                        {
                            debris.SetValue(FrameworkElement.TagProperty, debrisState + 1);
                        }
                    }
                }
            }
        }

        private void AnimateBuildingChunkDecal()
        {
            buildingElapsedMilliseconds = buildingElapsedMilliseconds + elapsedMilliseconds;

            //if (buildingTimer.ElapsedMilliseconds > 160)
            if (buildingElapsedMilliseconds > 160)
            {
                //buildingTimer.Reset();
                //buildingTimer.Start();
                buildingElapsedMilliseconds = 0;

                for (int i = 0; i < buildingBlocks.Count(); i++)
                {
                    int blockCondition = (int)buildingBlockDecals[i].GetValue(FrameworkElement.TagProperty);

                    if (buildingBlockDecals[i].Fill != buildingBlockAnimationFrames[timeCode][blockCondition])
                    {
                        if (blockCondition == 5)
                        {
                            buildingBlockDecals[i].Fill = null;
                            // when we're ready to punch a hole in the building, we'll choose a random hole to set the block to

                            // set the random hole value
                            //Random r = new Random();
                            // these are all the possible indexes for holes in the sprite sheet
                            //int rInt = r.Next(5, 15);
                            //buildingBlocks[i].Fill = buildingBlockAnimationFrames[timeCode][rInt];
                            buildingBlockDecals[i].SetValue(FrameworkElement.TagProperty, 6);
                        }

                        // if we're still in the process of punching a hole in the building, we're going to increase the frames by 1
                        if (blockCondition > 1 && blockCondition <= 4)
                        {
                            // we want to animate the decal and not the actual chunk
                            buildingBlockDecals[i].Fill = buildingBlockAnimationFrames[timeCode][blockCondition];

                            buildingBlockDecals[i].SetValue(FrameworkElement.TagProperty, blockCondition + 1);
                        }
                    }
                }
            }
        }
        
        private double EvenOutNumber(double num)
        {
            double newNumber = Math.Ceiling(num);

            if (newNumber % 2 != 0)
            {
                newNumber = newNumber + 1;
            }

            return newNumber;
        }

        
                
        private void UpdateClock()
        {
            if (showClock == true && !shutdownSequenceActivated)
            {
                DateTime rightNow = DateTime.Now;
                
                string hour = rightNow.ToString("hh", System.Globalization.CultureInfo.InvariantCulture);

                string hour1 = hour.Substring(0, 1);
                string hour2 = hour.Substring(1, 1);
                
                string minutes = rightNow.ToString("mm", System.Globalization.CultureInfo.InvariantCulture);

                string minute1 = minutes.Substring(0, 1);
                string minute2 = minutes.Substring(1, 1);
                
                string meridiem = rightNow.ToString("tt", System.Globalization.CultureInfo.InvariantCulture);

                if (hour1 != lastHour1)
                {
                    HourOneRectangle.Fill = numbers[hour1 + "0"];
                }

                if (hour2 != lastHour2)
                {
                    HourTwoRectangle.Fill = numbers[hour2 + "0"];
                }
                
                if (minute1 != lastMinute1)
                {
                    MinuteOneRectangle.Fill = numbers[minute1 + "0"];
                }
                
                if (minute2 != lastMinute2)
                {
                    MinuteTwoRectangle.Fill = numbers[minute2 + "0"];
                }
                
                if (meridiem != lastMeridiem)
                {
                    TwelveHourOneRectangle.Fill = numbers[meridiem];
                }

                // if we're calling update clock, we obviously want to show it
                if (ClockCanvas.Visibility == Visibility.Hidden || ClockCanvas.Visibility == Visibility.Collapsed)
                {
                    ClockCanvas.Visibility = Visibility.Visible;
                }

                lastHour1 = hour1;
                lastHour2 = hour2;
                lastMinute1 = minute1;
                lastMinute2 = minute2;
                lastMeridiem = meridiem;
            }
        }

        private void SetUIDimensions()
        {
            VsOverlay.Width = width;
            VsOverlay.Height = height;

            KOCountdown.Width = Math.Ceiling(1219 * resolutionXMultiplier);
            KOCountdown.Height = Math.Ceiling(146 * resolutionXMultiplier);

            PressStartRectangle.Width = Math.Ceiling(414 * resolutionXMultiplier);
            PressStartRectangle.Height = Math.Ceiling(67 * resolutionXMultiplier);

            double startTopPosition = ((screenBackgroundTopPosition - 1) - PressStartRectangle.Height) / 2;
            Canvas.SetRight(PressStartRectangle, startTopPosition);
            
            switch (surroundPosition)
            {
                case BigBlue.Models.SurroundPosition.Up:
                case BigBlue.Models.SurroundPosition.Down:
                    double topOffset = surroundGameListWidthOffset + ((height - surroundGameListWidthOffset) / 2);
                    double quarterOffset = surroundGameListWidthOffset + ((height - surroundGameListWidthOffset) / 4);

                    NewChallengerText.Margin = new Thickness((width / 2) - (Math.Ceiling(293 * resolutionXMultiplier)), topOffset - (Math.Ceiling(67 * resolutionXMultiplier)), 0, 0);
                    FightRectangle.Margin = new Thickness((width / 2) - (Math.Ceiling(118 * resolutionXMultiplier)), topOffset - (Math.Ceiling(43 * resolutionXMultiplier)), 0, 0);
                    DrawGameRectangle.Margin = new Thickness((width / 2) - (Math.Ceiling(110.5 * resolutionXMultiplier)), topOffset - (Math.Ceiling(63 * resolutionXMultiplier)), 0, 0);
                    DoubleKODecision.Margin = new Thickness((width / 2) - (Math.Ceiling(120 * resolutionXMultiplier)), topOffset - (Math.Ceiling(72.5 * resolutionXMultiplier)), 0, 0);
                    WinnerRectangle.Margin = new Thickness((width / 2) - (Math.Ceiling(330 * resolutionXMultiplier)), quarterOffset - (Math.Ceiling(24 * resolutionXMultiplier)), 0, 0);
                    double screen1Height = surroundGameListWidthOffset;
                    Canvas.SetTop(KOCountdown, screen1Height);
                    KOCountdown.Margin = new Thickness((width / 2) - (Math.Ceiling(608 * resolutionXMultiplier)), (Math.Ceiling(24 * resolutionXMultiplier)), 0, 0);

                    Canvas.SetTop(PressStartRectangle, surroundGameListWidthOffset + startTopPosition);

                    break;
                case BigBlue.Models.SurroundPosition.Left:
                case BigBlue.Models.SurroundPosition.Right:
                    NewChallengerText.Margin = new Thickness((surroundGameListWidthOffset / 2) - (Math.Ceiling(293 * resolutionXMultiplier)), ((height / 2) - (Math.Ceiling(67 * resolutionXMultiplier))), 0, 0);
                    FightRectangle.Margin = new Thickness((surroundGameListWidthOffset / 2) - (Math.Ceiling(118 * resolutionXMultiplier)), (height / 2) - (Math.Ceiling(43 * resolutionXMultiplier)), 0, 0);
                    DrawGameRectangle.Margin = new Thickness((surroundGameListWidthOffset / 2) - (Math.Ceiling(110.5 * resolutionXMultiplier)), (height / 2) - (Math.Ceiling(63 * resolutionXMultiplier)), 0, 0);
                    DoubleKODecision.Margin = new Thickness((surroundGameListWidthOffset / 2) - (Math.Ceiling(120 * resolutionXMultiplier)), (height / 2) - (Math.Ceiling(72.5 * resolutionXMultiplier)), 0, 0);
                    WinnerRectangle.Margin = new Thickness((surroundGameListWidthOffset / 2) - (Math.Ceiling(330 * resolutionXMultiplier)), (height / 4) - (Math.Ceiling(24 * resolutionXMultiplier)), 0, 0);
                    KOCountdown.Margin = new Thickness((surroundGameListWidthOffset / 2) - (Math.Ceiling(608 * resolutionXMultiplier)), (Math.Ceiling(24 * resolutionXMultiplier)), 0, 0);
                    Canvas.SetTop(PressStartRectangle, startTopPosition);
                    break;
                default:
                    double defaultTopOffset = height / 2;
                    NewChallengerText.Margin = new Thickness((width / 2) - (Math.Ceiling(293 * resolutionXMultiplier)), (defaultTopOffset - (Math.Ceiling(67 * resolutionXMultiplier))), 0, 0);
                    FightRectangle.Margin = new Thickness((width / 2) - (Math.Ceiling(118 * resolutionXMultiplier)), defaultTopOffset - (Math.Ceiling(43 * resolutionXMultiplier)), 0, 0);
                    DrawGameRectangle.Margin = new Thickness((width / 2) - (Math.Ceiling(110.5 * resolutionXMultiplier)), defaultTopOffset - (Math.Ceiling(63 * resolutionXMultiplier)), 0, 0);
                    DoubleKODecision.Margin = new Thickness((width / 2) - (Math.Ceiling(120 * resolutionXMultiplier)), defaultTopOffset - (Math.Ceiling(72.5 * resolutionXMultiplier)), 0, 0);
                    WinnerRectangle.Margin = new Thickness((width / 2) - (Math.Ceiling(330 * resolutionXMultiplier)), (height / 4) - (Math.Ceiling(24 * resolutionXMultiplier)), 0, 0);
                    KOCountdown.Margin = new Thickness((width / 2) - (Math.Ceiling(608 * resolutionXMultiplier)), (Math.Ceiling(24 * resolutionXMultiplier)), 0, 0);
                    Canvas.SetTop(PressStartRectangle, startTopPosition);
                    break;
            }
                        
            //Canvas.SetTop(LifeBars, Math.Ceiling(4 * resolutionXMultiplier));
            Canvas.SetTop(BarBorder, Math.Floor(15 * resolutionXMultiplier));

            double barBorderWidth = Math.Ceiling(1216 * resolutionXMultiplier);
            double barBorderHeight = Math.Ceiling(52 * resolutionXMultiplier);

            BarBorder.Width = barBorderWidth;
            BarBorder.Height = barBorderHeight;

            double lifeBarSideBorderWidth = Math.Ceiling(4 * resolutionXMultiplier);
            double lifeBarTopBottomBorderWidth = Math.Ceiling(5 * resolutionXMultiplier);

            BarBorder.BorderThickness = new Thickness(lifeBarSideBorderWidth, lifeBarTopBottomBorderWidth, lifeBarSideBorderWidth, lifeBarTopBottomBorderWidth);

            // set the width of the libars
            lifeBarWidth = Math.Ceiling(544 * resolutionXMultiplier);

            // set the width the life bar's at when the characters are at low health
            lowHealthWidth = 182 * resolutionXMultiplier;

            double lifeBarHeight = barBorderHeight - (lifeBarTopBottomBorderWidth * 2);

            BarBorderBackground.Width = barBorderWidth - (lifeBarSideBorderWidth * 2);
            BarBorderBackground.Height = lifeBarHeight;

            OneContainer.Width = lifeBarWidth;
            OneContainer.Height = lifeBarHeight;
            One.Width = lifeBarWidth;
            One.Height = lifeBarHeight;

            TwoContainer.Width = lifeBarWidth;
            TwoContainer.Height = lifeBarHeight;
            Two.Width = lifeBarWidth;
            Two.Height = lifeBarHeight;

            KO.Width = Math.Floor(121 * resolutionXMultiplier);
            KO.Height = Math.Floor(72 * resolutionXMultiplier);
            Canvas.SetLeft(KO, EvenOutNumber(548 * resolutionXMultiplier));

            FirstDigit.Width = Math.Ceiling(48 * resolutionXMultiplier);
            FirstDigit.Height = Math.Ceiling(68 * resolutionXMultiplier);
            Canvas.SetLeft(FirstDigit, Math.Ceiling(556 * resolutionXMultiplier));

            SecondDigit.Width = Math.Ceiling(48 * resolutionXMultiplier);
            SecondDigit.Height = Math.Ceiling(68 * resolutionXMultiplier);
            Canvas.SetRight(SecondDigit, Math.Ceiling(558 * resolutionXMultiplier));

            OneName.Width = Math.Ceiling(211 * resolutionXMultiplier);
            OneName.Height = Math.Ceiling(44 * resolutionXMultiplier);
            Canvas.SetLeft(OneName, Math.Ceiling(4 * resolutionXMultiplier));
            Canvas.SetTop(OneName, Math.Ceiling(77 * resolutionXMultiplier));

            TwoName.Width = Math.Ceiling(211 * resolutionXMultiplier);
            TwoName.Height = Math.Ceiling(44 * resolutionXMultiplier);
            Canvas.SetRight(TwoName, Math.Ceiling(4 * resolutionXMultiplier));
            Canvas.SetTop(TwoName, Math.Ceiling(77 * resolutionXMultiplier));
                        

            //ClockCanvas.Margin = new Thickness(startTopPosition, height - (68 + startTopPosition), 0, 0);
            
            Canvas.SetZIndex(ClockCanvas, 101);
            Canvas.SetLeft(ClockCanvas, startTopPosition);
            Canvas.SetBottom(ClockCanvas, startTopPosition);
            //MessageBox.Show(startTopPosition.ToString());


            NewChallengerText.Width = Math.Ceiling(586 * resolutionXMultiplier);
            NewChallengerText.Height = Math.Ceiling(135 * resolutionXMultiplier);
            NewChallengerImage.Viewport = new Rect(0, 0, Math.Ceiling(586 * resolutionXMultiplier), Math.Ceiling(135 * resolutionXMultiplier));
        }

        private void SetResolutionSpecificDimensions()
        {
            SetSkyDimensions();
            ProvisionStatic();
            SetCharacterDimensions();
            SetBackgroundDimensions();
            SetUIDimensions();
            SetMainMenuDimensions(MainMenu, whiteBrush);

            // at lower resolutions, using -10 --> 10 is too extreme for the screen shake
            screenShakeStartRange = Convert.ToInt32(-10 * resolutionXMultiplier);
            screenShakeEndRange = Convert.ToInt32(10 * resolutionXMultiplier);
        }
        
        private void SpawnRampageMonster(bool autoStart)
        {
            if (GeorgeDamageDecal.Visibility == System.Windows.Visibility.Visible)
            {
                GeorgeDamageDecal.Visibility = System.Windows.Visibility.Hidden;
            }

            One.Width = lifeBarWidth;

            if (georgeState == RampageState.falling)
            {
                GeorgeFallingStoryBoard.Stop();
            }

            if (georgeState == RampageState.climbing)
            {
                GeorgeClimbingStoryBoard.Stop();
            }
            
            // reset george's state to climbing and make sure that his spectating stopwatch timer is reset
            rampageFrameToDisplay = 0;
            georgeState = RampageState.climbing;
            spectateStopWatch.Reset();

            climbingUp = true;
            
            Canvas.SetTop(RampageMonsterRectangle, height);
            Canvas.SetTop(GeorgeDamageDecal, height);

            if (autoStart == true)
            {
                GeorgeClimbingStoryBoard.Begin();
            }
        }

        private void AnimateSky()
        {
            DoubleAnimation skyAnimation = new DoubleAnimation();
            skyAnimation.From = 0;
            skyAnimation.To = -width;
            skyAnimation.Duration = new Duration(TimeSpan.FromSeconds(30));
            skyAnimation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTarget(skyAnimation, SkyGradient1);
            Storyboard.SetTargetProperty(skyAnimation, new PropertyPath("(Canvas.Left)"));
            skyAnimation.Freeze();


            SkyStoryBoard.Children.Add(skyAnimation);
            SkyStoryBoard.Freeze();

            SkyStoryBoard.Begin();

            DoubleAnimation sky2Animation = new DoubleAnimation();
            sky2Animation.From = 0;
            sky2Animation.To = -width;
            sky2Animation.Duration = new Duration(TimeSpan.FromSeconds(50));
            sky2Animation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTarget(sky2Animation, SkyGradient2);
            Storyboard.SetTargetProperty(sky2Animation, new PropertyPath("(Canvas.Left)"));
            sky2Animation.Freeze();

            Sky2StoryBoard.Children.Add(sky2Animation);
            Sky2StoryBoard.Freeze();

            Sky2StoryBoard.Begin();

            DoubleAnimation sky3Animation = new DoubleAnimation();
            sky3Animation.From = 0;
            sky3Animation.To = -width;
            sky3Animation.Duration = new Duration(TimeSpan.FromSeconds(70));
            sky3Animation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTarget(sky3Animation, SkyGradient3);
            Storyboard.SetTargetProperty(sky3Animation, new PropertyPath("(Canvas.Left)"));

            sky3Animation.Freeze();

            Sky3StoryBoard.Children.Add(sky3Animation);
            Sky3StoryBoard.Freeze();

            Sky3StoryBoard.Begin();

            DoubleAnimation sky4Animation = new DoubleAnimation();
            sky4Animation.From = 0;
            sky4Animation.To = -width;
            sky4Animation.Duration = new Duration(TimeSpan.FromSeconds(90));
            sky4Animation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTarget(sky4Animation, SkyGradient4);
            Storyboard.SetTargetProperty(sky4Animation, new PropertyPath("(Canvas.Left)"));

            sky4Animation.Freeze();

            Sky4StoryBoard.Children.Add(sky4Animation);
            Sky4StoryBoard.Freeze();

            Sky4StoryBoard.Begin();
        }

        private void ProvisionLaser()
        {
            laserRects.Add(new Rect(316, 335, 60, 16));
        }

        private void ProvisionWinnerText()
        {
            winnerTextRects.Add(new Rect(0, 0, 661, 48));
            winnerTextRects.Add(new Rect(0, 52, 661, 48));
            winnerTextRects.Add(new Rect(0, 104, 661, 48));
            winnerTextRects.Add(new Rect(0, 156, 661, 48));
            winnerTextRects.Add(new Rect(0, 208, 661, 48));
            winnerTextRects.Add(new Rect(0, 260, 661, 48));
            winnerTextRects.Add(new Rect(0, 312, 661, 48));
            winnerTextRects.Add(new Rect(0, 364, 661, 48));
        }

        private void ProvisionKnockOut()
        {
            koRects.Add(new Rect(0, 0, 120, 73));
            koRects.Add(new Rect(124, 0, 120, 73));
        }

        List<List<ImageBrush>> fightersAnimationFrames = new List<List<ImageBrush>>(3);
        Rect[] fighterRects = new Rect[6];

        private void ProvisionFighters()
        {
            for (int i = 0; i < 4; i++)
            {
                List<ImageBrush> fightersBrushes = new List<ImageBrush>(6);
                fightersAnimationFrames.Add(fightersBrushes);
            }

            // fighter images
            BitmapImage fightersImageDay = BigBlue.ImageLoading.loadImage("fighters.png", integerMultiplier);
            BitmapImage fightersImageSunset = BigBlue.ImageLoading.loadImage("fighters_sunset.png", integerMultiplier);
            BitmapImage fightersImageNight = BigBlue.ImageLoading.loadImage("fighters_night.png", integerMultiplier);
            BitmapImage fightersImageDawn = BigBlue.ImageLoading.loadImage("fighters_dawn.png", integerMultiplier);


            fighterRects[0] = new Rect(0, 0, 1189, 694); // frame #1
            fighterRects[1] = new Rect(0, 698, 1189, 694); // frame #2
            fighterRects[2] = new Rect(0, 1396, 1189, 694); // frame #3
            fighterRects[3] = new Rect(0, 2094, 1189, 694); // frame #4
            fighterRects[4] = new Rect(0, 2792, 1189, 694); // frame #5
            fighterRects[5] = new Rect(0, 2094, 1189, 694); // frame #6

            foreach (Rect f in fighterRects)
            {
                fightersAnimationFrames[0].Add(BigBlue.ImageLoading.loadAnimationFrame(fightersImageDay, f, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                fightersAnimationFrames[1].Add(BigBlue.ImageLoading.loadAnimationFrame(fightersImageSunset, f, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                fightersAnimationFrames[2].Add(BigBlue.ImageLoading.loadAnimationFrame(fightersImageNight, f, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                fightersAnimationFrames[3].Add(BigBlue.ImageLoading.loadAnimationFrame(fightersImageDawn, f, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
            }
        }

        private void ProvisionBuildings()
        {
            // 0
            buildingRects.Add(new Rect(-300, -300, 0, 0));
            // 1
            buildingRects.Add(new Rect(0, 0, 124, 91));
            // 2
            buildingRects.Add(new Rect(0, 95, 124, 91));
            // 3
            buildingRects.Add(new Rect(0, 190, 124, 91));
            // 4
            buildingRects.Add(new Rect(0, 285, 124, 91));
            // 5
            buildingRects.Add(new Rect(0, 380, 124, 91));
            // 6
            buildingRects.Add(new Rect(128, 0, 124, 91));
            // 7
            buildingRects.Add(new Rect(128, 95, 124, 91));
            // 8
            buildingRects.Add(new Rect(128, 190, 124, 91));
            // 9
            buildingRects.Add(new Rect(128, 285, 124, 91));
            // 10
            buildingRects.Add(new Rect(128, 380, 124, 91));
            // 11
            buildingRects.Add(new Rect(128, 0, 124, 91));
            // 12
            buildingRects.Add(new Rect(256, 95, 124, 91));
            // 13
            buildingRects.Add(new Rect(256, 190, 124, 91));
            // 14
            buildingRects.Add(new Rect(256, 285, 124, 91));
            // 15
            buildingRects.Add(new Rect(256, 380, 124, 91));

            // debris animations
            // 16
            buildingRects.Add(new Rect(0, 475, 220, 72));
            // 17
            buildingRects.Add(new Rect(0, 551, 220, 72));
            // 18
            buildingRects.Add(new Rect(0, 627, 220, 72));
            // 19
            buildingRects.Add(new Rect(0, 703, 220, 72));
        }
        
        private void ProvisionPlasma()
        {
            plasmaRects.Add(new Rect(0, 0, 120, 123));
            plasmaRects.Add(new Rect(124, 0, 120, 123));
            plasmaRects.Add(new Rect(246, 0, 120, 123));
        }
        
        Dictionary<int, ImageBrush> staticBrushes = new Dictionary<int, ImageBrush>(2);
        
        private void ProvisionStatic()
        {
            const int baseWidth = 60;
            const int baseHeight = 78;
            
            double mainWindowAdjustedWidth = baseWidth * resolutionXMultiplier;
            double mainWindowAdjustedHeight = baseHeight * resolutionXMultiplier;
            
            BitmapImage staticImage1 = BigBlue.ImageLoading.loadImage("static1.png", integerMultiplier);
            BitmapImage staticImage2 = BigBlue.ImageLoading.loadImage("static2.png", integerMultiplier);
                        
            Rect mainWindowStaticViewPort = new Rect(0, 0, mainWindowAdjustedWidth, mainWindowAdjustedHeight);
            
            staticBrushes[0] = BigBlue.ImageLoading.loadTiledAnimationFrame(staticImage1, mainWindowStaticViewPort, BrushMappingMode.Absolute, TileMode.Tile, integerMultiplier);
            staticBrushes[1] = BigBlue.ImageLoading.loadTiledAnimationFrame(staticImage2, mainWindowStaticViewPort, BrushMappingMode.Absolute, TileMode.Tile, integerMultiplier);
        }

        private void ProvisionKnifeAnimation()
        {
            DoubleAnimation knifeAnimation = new DoubleAnimation();
            knifeAnimation.From = -KnifeRectangle.Height;

            // going to have to do a separate destination for the portrait mode
            knifeAnimation.To = knifeTableOffset;
            knifeAnimation.SpeedRatio = 1.0;
            knifeAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            knifeAnimation.AutoReverse = false;

            knifeAnimation.Completed += KnifeAnimation_Completed;

            Storyboard.SetTarget(knifeAnimation, KnifeRectangle);
            Storyboard.SetTargetProperty(knifeAnimation, new PropertyPath("(Canvas.Top)"));

            knifeAnimation.Freeze();

            KnifeStoryboard.Children.Add(knifeAnimation);

            KnifeStoryboard.Freeze();
        }

        List<ImageBrush> haggarAnimationFrames = new List<ImageBrush>(7);

        private void ProvisionHaggar()
        {
            ProvisionKnifeAnimation();

            //HaggarImage.ImageSource = haggarImage;
                        
            Video.Width = snapShotWidth;
            Video.Height = snapShotHeight;
            VideoElement.Width = snapShotWidth;
            VideoElement.Height = snapShotHeight;

            int haggarAspectIndex = portraitModeIndex;

            if (portraitModeIndex == 1)
            {
                // haggar images
                //BitmapImage haggarImage = BigBlue.ImageLoading.loadImage("haggar" + portraitModeIndex + ".png", integerMultiplier);

                // knife image
                knifeImage = BigBlue.ImageLoading.loadImage("knife1.png", integerMultiplier);
                KnifeImage.ImageSource = knifeImage;

                ProvisionPortraitFuse();
            }
            else
            {
                if (aspectRatioIndex == 2)
                {
                    knifeImage = BigBlue.ImageLoading.loadImage("knife2.png", integerMultiplier);
                    KnifeImage.ImageSource = knifeImage;

                    haggarAspectIndex = aspectRatioIndex;
                    ProvisionWideScreenFuse();
                }
                else
                {
                    knifeImage = BigBlue.ImageLoading.loadImage("knife0.png", integerMultiplier);
                    KnifeImage.ImageSource = knifeImage;
                    ProvisionFuse();
                }
            }

            // load the fuse graphics
            igniteImageBrushes.Add(BigBlue.ImageLoading.loadImageBrush("ignite" + haggarAspectIndex + "a.png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, BrushMappingMode.Absolute, nullRect, BrushMappingMode.Absolute, true, integerMultiplier));
            igniteImageBrushes.Add(BigBlue.ImageLoading.loadImageBrush("ignite" + haggarAspectIndex + "b.png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, BrushMappingMode.Absolute, nullRect, BrushMappingMode.Absolute, true, integerMultiplier));

            // haggar foreground image to cover up the knife when it sticks into the table
            BitmapImage haggarForegroundImage = BigBlue.ImageLoading.loadImage("haggarforeground" + haggarAspectIndex + ".png", integerMultiplier);
            HaggarForegroundImage.ImageSource = haggarForegroundImage;

            // set the background image for haggar
            HaggarBackgroundRectangle.Fill = BigBlue.ImageLoading.loadImageBrush("haggar" + haggarAspectIndex + ".png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, BrushMappingMode.Absolute, nullRect, BrushMappingMode.Absolute, true, integerMultiplier);

            haggarAnimationFrames.Add(BigBlue.ImageLoading.loadImageBrush("haggar" + haggarAspectIndex + "a.png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, BrushMappingMode.Absolute, nullRect, BrushMappingMode.Absolute, true, integerMultiplier));
            haggarAnimationFrames.Add(BigBlue.ImageLoading.loadImageBrush("haggar" + haggarAspectIndex + "b.png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, BrushMappingMode.Absolute, nullRect, BrushMappingMode.Absolute, true, integerMultiplier));
            haggarAnimationFrames.Add(BigBlue.ImageLoading.loadImageBrush("haggar" + haggarAspectIndex + "c.png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, BrushMappingMode.Absolute, nullRect, BrushMappingMode.Absolute, true, integerMultiplier));
            haggarAnimationFrames.Add(BigBlue.ImageLoading.loadImageBrush("haggar" + haggarAspectIndex + "b.png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, BrushMappingMode.Absolute, nullRect, BrushMappingMode.Absolute, true, integerMultiplier));
            haggarAnimationFrames.Add(BigBlue.ImageLoading.loadImageBrush("haggar" + haggarAspectIndex + "c.png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, BrushMappingMode.Absolute, nullRect, BrushMappingMode.Absolute, true, integerMultiplier));
            haggarAnimationFrames.Add(BigBlue.ImageLoading.loadImageBrush("haggar" + haggarAspectIndex + "b.png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, BrushMappingMode.Absolute, nullRect, BrushMappingMode.Absolute, true, integerMultiplier));
            haggarAnimationFrames.Add(BigBlue.ImageLoading.loadImageBrush("haggar" + haggarAspectIndex + "d.png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, BrushMappingMode.Absolute, nullRect, BrushMappingMode.Absolute, true, integerMultiplier));
        }

        private void ProvisionFuse()
        {
            fusePoints.Add(new Point((365 - 46) * resolutionXMultiplier, (421 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((365 - 46) * resolutionXMultiplier, (420 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((365 - 46) * resolutionXMultiplier, (419 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((365 - 46) * resolutionXMultiplier, (418 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((365 - 46) * resolutionXMultiplier, (417 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((364 - 46) * resolutionXMultiplier, (416 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((364 - 46) * resolutionXMultiplier, (415 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((364 - 46) * resolutionXMultiplier, (414 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((363 - 46) * resolutionXMultiplier, (413 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((363 - 46) * resolutionXMultiplier, (412 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((362 - 46) * resolutionXMultiplier, (411 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((362 - 46) * resolutionXMultiplier, (410 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((361 - 46) * resolutionXMultiplier, (409 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((361 - 46) * resolutionXMultiplier, (408 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((361 - 46) * resolutionXMultiplier, (407 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((360 - 46) * resolutionXMultiplier, (406 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((360 - 46) * resolutionXMultiplier, (405 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((360 - 46) * resolutionXMultiplier, (404 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((359 - 46) * resolutionXMultiplier, (403 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((359 - 46) * resolutionXMultiplier, (402 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((359 - 46) * resolutionXMultiplier, (401 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((359 - 46) * resolutionXMultiplier, (400 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((358 - 46) * resolutionXMultiplier, (399 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((358 - 46) * resolutionXMultiplier, (398 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((358 - 46) * resolutionXMultiplier, (397 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((358 - 46) * resolutionXMultiplier, (396 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((358 - 46) * resolutionXMultiplier, (395 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((357 - 46) * resolutionXMultiplier, (394 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((357 - 46) * resolutionXMultiplier, (393 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((357 - 46) * resolutionXMultiplier, (392 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((357 - 46) * resolutionXMultiplier, (391 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((357 - 46) * resolutionXMultiplier, (390 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((357 - 46) * resolutionXMultiplier, (389 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((357 - 46) * resolutionXMultiplier, (388 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((356 - 46) * resolutionXMultiplier, (387 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((356 - 46) * resolutionXMultiplier, (386 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((356 - 46) * resolutionXMultiplier, (385 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((355 - 46) * resolutionXMultiplier, (384 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((355 - 46) * resolutionXMultiplier, (383 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((355 - 46) * resolutionXMultiplier, (382 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((355 - 46) * resolutionXMultiplier, (381 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((355 - 46) * resolutionXMultiplier, (380 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((354 - 46) * resolutionXMultiplier, (379 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((354 - 46) * resolutionXMultiplier, (378 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((354 - 46) * resolutionXMultiplier, (377 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((354 - 46) * resolutionXMultiplier, (376 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((354 - 46) * resolutionXMultiplier, (375 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((354 - 46) * resolutionXMultiplier, (374 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((354 - 46) * resolutionXMultiplier, (373 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((353 - 46) * resolutionXMultiplier, (372 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((353 - 46) * resolutionXMultiplier, (371 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((353 - 46) * resolutionXMultiplier, (370 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((353 - 46) * resolutionXMultiplier, (369 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((353 - 46) * resolutionXMultiplier, (368 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((353 - 46) * resolutionXMultiplier, (367 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((353 - 46) * resolutionXMultiplier, (366 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((353 - 46) * resolutionXMultiplier, (365 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((353 - 46) * resolutionXMultiplier, (364 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((353 - 46) * resolutionXMultiplier, (363 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((353 - 46) * resolutionXMultiplier, (362 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((352 - 46) * resolutionXMultiplier, (361 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((352 - 46) * resolutionXMultiplier, (360 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((352 - 46) * resolutionXMultiplier, (359 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((352 - 46) * resolutionXMultiplier, (358 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((352 - 46) * resolutionXMultiplier, (357 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((352 - 46) * resolutionXMultiplier, (356 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((352 - 46) * resolutionXMultiplier, (355 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((352 - 46) * resolutionXMultiplier, (354 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((352 - 46) * resolutionXMultiplier, (353 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((352 - 46) * resolutionXMultiplier, (352 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (351 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (350 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (349 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (348 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (347 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (346 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (345 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (344 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (343 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (342 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (341 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (340 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (339 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (338 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (337 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (336 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (335 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (334 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (333 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (332 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (331 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (330 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (329 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (328 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (327 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (326 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (325 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (324 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (323 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (322 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (321 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (320 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (319 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (318 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (317 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (316 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (315 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (314 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (313 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (312 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (311 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (310 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (309 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (308 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (307 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (306 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (305 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (304 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (303 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((347 - 46) * resolutionXMultiplier, (302 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((347 - 46) * resolutionXMultiplier, (301 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((347 - 46) * resolutionXMultiplier, (300 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((347 - 46) * resolutionXMultiplier, (299 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((347 - 46) * resolutionXMultiplier, (298 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((347 - 46) * resolutionXMultiplier, (297 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (296 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (295 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (294 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (293 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (292 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (291 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (290 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (289 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (288 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (287 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (286 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (285 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (284 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (283 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (282 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (281 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (280 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (279 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((351 - 46) * resolutionXMultiplier, (278 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (277 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (276 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (275 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((350 - 46) * resolutionXMultiplier, (274 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (273 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (272 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((349 - 46) * resolutionXMultiplier, (271 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (270 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((348 - 46) * resolutionXMultiplier, (269 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((347 - 46) * resolutionXMultiplier, (268 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((347 - 46) * resolutionXMultiplier, (267 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((347 - 46) * resolutionXMultiplier, (266 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((347 - 46) * resolutionXMultiplier, (265 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((347 - 46) * resolutionXMultiplier, (264 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((346 - 46) * resolutionXMultiplier, (263 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((346 - 46) * resolutionXMultiplier, (262 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((346 - 46) * resolutionXMultiplier, (261 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((345 - 46) * resolutionXMultiplier, (260 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((345 - 46) * resolutionXMultiplier, (259 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((345 - 46) * resolutionXMultiplier, (258 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((344 - 46) * resolutionXMultiplier, (257 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((344 - 46) * resolutionXMultiplier, (256 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((343 - 46) * resolutionXMultiplier, (255 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((342 - 46) * resolutionXMultiplier, (254 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((341 - 46) * resolutionXMultiplier, (253 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((340 - 46) * resolutionXMultiplier, (252 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((339 - 46) * resolutionXMultiplier, (251 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((338 - 46) * resolutionXMultiplier, (251 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((337 - 46) * resolutionXMultiplier, (250 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((336 - 46) * resolutionXMultiplier, (250 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((335 - 46) * resolutionXMultiplier, (249 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((334 - 46) * resolutionXMultiplier, (249 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((333 - 46) * resolutionXMultiplier, (249 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((332 - 46) * resolutionXMultiplier, (248 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((331 - 46) * resolutionXMultiplier, (248 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((330 - 46) * resolutionXMultiplier, (248 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((329 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((328 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((327 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((326 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((325 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((324 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((323 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((322 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((321 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((320 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((319 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((318 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((317 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((316 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((315 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((314 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((313 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((312 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((311 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((310 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((309 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((308 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((307 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((306 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((305 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((304 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((303 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((302 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((301 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((300 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((299 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((298 - 46) * resolutionXMultiplier, (247 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((298 - 46) * resolutionXMultiplier, (248 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((297 - 46) * resolutionXMultiplier, (248 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((296 - 46) * resolutionXMultiplier, (248 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((295 - 46) * resolutionXMultiplier, (248 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((294 - 46) * resolutionXMultiplier, (248 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((293 - 46) * resolutionXMultiplier, (248 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((292 - 46) * resolutionXMultiplier, (248 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((292 - 46) * resolutionXMultiplier, (249 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((291 - 46) * resolutionXMultiplier, (249 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((290 - 46) * resolutionXMultiplier, (249 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((289 - 46) * resolutionXMultiplier, (249 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((289 - 46) * resolutionXMultiplier, (250 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((288 - 46) * resolutionXMultiplier, (250 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((287 - 46) * resolutionXMultiplier, (250 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((287 - 46) * resolutionXMultiplier, (251 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((286 - 46) * resolutionXMultiplier, (251 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((285 - 46) * resolutionXMultiplier, (251 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((285 - 46) * resolutionXMultiplier, (252 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((284 - 46) * resolutionXMultiplier, (252 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((283 - 46) * resolutionXMultiplier, (252 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((283 - 46) * resolutionXMultiplier, (253 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((282 - 46) * resolutionXMultiplier, (253 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((282 - 46) * resolutionXMultiplier, (254 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((282 - 46) * resolutionXMultiplier, (255 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((281 - 46) * resolutionXMultiplier, (255 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((281 - 46) * resolutionXMultiplier, (256 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((281 - 46) * resolutionXMultiplier, (257 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((281 - 46) * resolutionXMultiplier, (258 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((281 - 46) * resolutionXMultiplier, (259 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((281 - 46) * resolutionXMultiplier, (260 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((281 - 46) * resolutionXMultiplier, (261 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((281 - 46) * resolutionXMultiplier, (262 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((281 - 46) * resolutionXMultiplier, (263 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((280 - 46) * resolutionXMultiplier, (263 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((280 - 46) * resolutionXMultiplier, (264 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((280 - 46) * resolutionXMultiplier, (265 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((280 - 46) * resolutionXMultiplier, (266 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((280 - 46) * resolutionXMultiplier, (267 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((280 - 46) * resolutionXMultiplier, (268 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((280 - 46) * resolutionXMultiplier, (269 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((280 - 46) * resolutionXMultiplier, (270 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((280 - 46) * resolutionXMultiplier, (271 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((281 - 46) * resolutionXMultiplier, (271 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((281 - 46) * resolutionXMultiplier, (272 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((280 - 46) * resolutionXMultiplier, (273 - 64) * resolutionXMultiplier));
            fusePoints.Add(new Point((280 - 46) * resolutionXMultiplier, (274 - 64) * resolutionXMultiplier));
        }

        private void ProvisionWideScreenFuse()
        {
            // this should really be half of whatever the ignite rectangle is sized at instead of a magic number
            double xOffset = IgniteRectangle.Width / 2;
            double yOffset = IgniteRectangle.Height / 2;

            fusePoints.Add(new Point(((345 * resolutionXMultiplier) - xOffset), ((315 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((345 * resolutionXMultiplier) - xOffset), ((314 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((345 * resolutionXMultiplier) - xOffset), ((313 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((344 * resolutionXMultiplier) - xOffset), ((312 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((344 * resolutionXMultiplier) - xOffset), ((311 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((343 * resolutionXMultiplier) - xOffset), ((310 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((343 * resolutionXMultiplier) - xOffset), ((309 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((342 * resolutionXMultiplier) - xOffset), ((308 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((341 * resolutionXMultiplier) - xOffset), ((307 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((341 * resolutionXMultiplier) - xOffset), ((306 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((341 * resolutionXMultiplier) - xOffset), ((305 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((340 * resolutionXMultiplier) - xOffset), ((304 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((340 * resolutionXMultiplier) - xOffset), ((303 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((340 * resolutionXMultiplier) - xOffset), ((302 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((339 * resolutionXMultiplier) - xOffset), ((301 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((339 * resolutionXMultiplier) - xOffset), ((300 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((339 * resolutionXMultiplier) - xOffset), ((299 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((339 * resolutionXMultiplier) - xOffset), ((298 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((339 * resolutionXMultiplier) - xOffset), ((297 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((339 * resolutionXMultiplier) - xOffset), ((296 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((339 * resolutionXMultiplier) - xOffset), ((295 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((339 * resolutionXMultiplier) - xOffset), ((294 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((339 * resolutionXMultiplier) - xOffset), ((293 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((339 * resolutionXMultiplier) - xOffset), ((292 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((339 * resolutionXMultiplier) - xOffset), ((291 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((338 * resolutionXMultiplier) - xOffset), ((291 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((338 * resolutionXMultiplier) - xOffset), ((290 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((338 * resolutionXMultiplier) - xOffset), ((289 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((337 * resolutionXMultiplier) - xOffset), ((288 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((337 * resolutionXMultiplier) - xOffset), ((287 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((337 * resolutionXMultiplier) - xOffset), ((286 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((337 * resolutionXMultiplier) - xOffset), ((285 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((337 * resolutionXMultiplier) - xOffset), ((284 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((337 * resolutionXMultiplier) - xOffset), ((283 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((337 * resolutionXMultiplier) - xOffset), ((282 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((337 * resolutionXMultiplier) - xOffset), ((281 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((337 * resolutionXMultiplier) - xOffset), ((280 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((336 * resolutionXMultiplier) - xOffset), ((280 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((336 * resolutionXMultiplier) - xOffset), ((279 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((336 * resolutionXMultiplier) - xOffset), ((278 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((336 * resolutionXMultiplier) - xOffset), ((277 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((336 * resolutionXMultiplier) - xOffset), ((276 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((336 * resolutionXMultiplier) - xOffset), ((275 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((335 * resolutionXMultiplier) - xOffset), ((275 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((335 * resolutionXMultiplier) - xOffset), ((274 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((335 * resolutionXMultiplier) - xOffset), ((273 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((335 * resolutionXMultiplier) - xOffset), ((272 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((334 * resolutionXMultiplier) - xOffset), ((272 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((334 * resolutionXMultiplier) - xOffset), ((271 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((334 * resolutionXMultiplier) - xOffset), ((270 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((334 * resolutionXMultiplier) - xOffset), ((269 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((334 * resolutionXMultiplier) - xOffset), ((268 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((268 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((267 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((266 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((265 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((264 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((263 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((262 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((261 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((260 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((259 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((258 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((257 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((256 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((255 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((254 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((253 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((252 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((251 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((250 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((249 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((248 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((247 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((246 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((245 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((244 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((243 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((242 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((241 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((240 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((239 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((238 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((333 * resolutionXMultiplier) - xOffset), ((237 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((237 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((236 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((235 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((234 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((233 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((232 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((231 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((230 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((229 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((228 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((227 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((226 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((225 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((224 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((223 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((222 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((221 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((220 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((219 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((218 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((217 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((216 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((215 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((214 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((213 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((212 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((211 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((210 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((209 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((208 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((207 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((332 * resolutionXMultiplier) - xOffset), ((206 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((331 * resolutionXMultiplier) - xOffset), ((206 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((331 * resolutionXMultiplier) - xOffset), ((204 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((331 * resolutionXMultiplier) - xOffset), ((203 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((330 * resolutionXMultiplier) - xOffset), ((203 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((330 * resolutionXMultiplier) - xOffset), ((202 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((330 * resolutionXMultiplier) - xOffset), ((201 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((329 * resolutionXMultiplier) - xOffset), ((201 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((329 * resolutionXMultiplier) - xOffset), ((200 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((329 * resolutionXMultiplier) - xOffset), ((199 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((329 * resolutionXMultiplier) - xOffset), ((198 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((329 * resolutionXMultiplier) - xOffset), ((197 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((329 * resolutionXMultiplier) - xOffset), ((196 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((329 * resolutionXMultiplier) - xOffset), ((195 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((329 * resolutionXMultiplier) - xOffset), ((194 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((328 * resolutionXMultiplier) - xOffset), ((194 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((328 * resolutionXMultiplier) - xOffset), ((193 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((328 * resolutionXMultiplier) - xOffset), ((192 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((328 * resolutionXMultiplier) - xOffset), ((191 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((327 * resolutionXMultiplier) - xOffset), ((191 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((327 * resolutionXMultiplier) - xOffset), ((190 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((327 * resolutionXMultiplier) - xOffset), ((189 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((326 * resolutionXMultiplier) - xOffset), ((189 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((325 * resolutionXMultiplier) - xOffset), ((189 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((325 * resolutionXMultiplier) - xOffset), ((188 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((324 * resolutionXMultiplier) - xOffset), ((188 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((323 * resolutionXMultiplier) - xOffset), ((188 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((323 * resolutionXMultiplier) - xOffset), ((187 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((322 * resolutionXMultiplier) - xOffset), ((187 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((321 * resolutionXMultiplier) - xOffset), ((187 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((320 * resolutionXMultiplier) - xOffset), ((187 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((319 * resolutionXMultiplier) - xOffset), ((187 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((319 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((318 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((317 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((316 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((315 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((314 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((313 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((312 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((311 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((310 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((310 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((309 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((308 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((307 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((306 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((305 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((304 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((303 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((302 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((301 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((300 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((299 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((298 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((297 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((296 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((295 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((294 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((293 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((292 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((291 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((290 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((289 * resolutionXMultiplier) - xOffset), ((185 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((289 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((288 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((287 * resolutionXMultiplier) - xOffset), ((186 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((287 * resolutionXMultiplier) - xOffset), ((187 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((286 * resolutionXMultiplier) - xOffset), ((187 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((286 * resolutionXMultiplier) - xOffset), ((188 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((286 * resolutionXMultiplier) - xOffset), ((189 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((285 * resolutionXMultiplier) - xOffset), ((189 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((285 * resolutionXMultiplier) - xOffset), ((190 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((284 * resolutionXMultiplier) - xOffset), ((190 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((284 * resolutionXMultiplier) - xOffset), ((191 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((284 * resolutionXMultiplier) - xOffset), ((192 * resolutionXMultiplier) - yOffset)));

            fusePoints.Add(new Point(((283 * resolutionXMultiplier) - xOffset), ((192 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((283 * resolutionXMultiplier) - xOffset), ((193 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((283 * resolutionXMultiplier) - xOffset), ((194 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((282 * resolutionXMultiplier) - xOffset), ((194 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((282 * resolutionXMultiplier) - xOffset), ((195 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((282 * resolutionXMultiplier) - xOffset), ((196 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((281 * resolutionXMultiplier) - xOffset), ((196 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((281 * resolutionXMultiplier) - xOffset), ((197 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((281 * resolutionXMultiplier) - xOffset), ((198 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((281 * resolutionXMultiplier) - xOffset), ((199 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((281 * resolutionXMultiplier) - xOffset), ((200 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((280 * resolutionXMultiplier) - xOffset), ((200 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((280 * resolutionXMultiplier) - xOffset), ((201 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((280 * resolutionXMultiplier) - xOffset), ((202 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((280 * resolutionXMultiplier) - xOffset), ((203 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((280 * resolutionXMultiplier) - xOffset), ((204 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((280 * resolutionXMultiplier) - xOffset), ((205 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((279 * resolutionXMultiplier) - xOffset), ((205 * resolutionXMultiplier) - yOffset)));


        }
        
        private void ProvisionPortraitFuse()
        {
            // this should really be half of whatever the ignite rectangle is sized at instead of a magic number
            double xOffset = IgniteRectangle.Width / 2;
            double yOffset = IgniteRectangle.Height / 2;
            
            fusePoints.Add(new Point(((287 * resolutionXMultiplier) - xOffset), ((561 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((287 * resolutionXMultiplier) - xOffset), ((560 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((287 * resolutionXMultiplier) - xOffset), ((559 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((287 * resolutionXMultiplier) - xOffset), ((558 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((287 * resolutionXMultiplier) - xOffset), ((557 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((287 * resolutionXMultiplier) - xOffset), ((556 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((286 * resolutionXMultiplier) - xOffset), ((556 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((286 * resolutionXMultiplier) - xOffset), ((555 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((286 * resolutionXMultiplier) - xOffset), ((554 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((286 * resolutionXMultiplier) - xOffset), ((553 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((285 * resolutionXMultiplier) - xOffset), ((553 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((285 * resolutionXMultiplier) - xOffset), ((552 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((285 * resolutionXMultiplier) - xOffset), ((551 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((284 * resolutionXMultiplier) - xOffset), ((551 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((284 * resolutionXMultiplier) - xOffset), ((549 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((284 * resolutionXMultiplier) - xOffset), ((548 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((283 * resolutionXMultiplier) - xOffset), ((548 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((283 * resolutionXMultiplier) - xOffset), ((547 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((283 * resolutionXMultiplier) - xOffset), ((546 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((282 * resolutionXMultiplier) - xOffset), ((546 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((282 * resolutionXMultiplier) - xOffset), ((545 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((282 * resolutionXMultiplier) - xOffset), ((544 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((281 * resolutionXMultiplier) - xOffset), ((544 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((281 * resolutionXMultiplier) - xOffset), ((543 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((281 * resolutionXMultiplier) - xOffset), ((542 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((281 * resolutionXMultiplier) - xOffset), ((541 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((280 * resolutionXMultiplier) - xOffset), ((541 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((280 * resolutionXMultiplier) - xOffset), ((540 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((280 * resolutionXMultiplier) - xOffset), ((538 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((279 * resolutionXMultiplier) - xOffset), ((538 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((279 * resolutionXMultiplier) - xOffset), ((537 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((279 * resolutionXMultiplier) - xOffset), ((536 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((279 * resolutionXMultiplier) - xOffset), ((535 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((278 * resolutionXMultiplier) - xOffset), ((535 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((278 * resolutionXMultiplier) - xOffset), ((534 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((278 * resolutionXMultiplier) - xOffset), ((533 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((278 * resolutionXMultiplier) - xOffset), ((532 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((278 * resolutionXMultiplier) - xOffset), ((531 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((278 * resolutionXMultiplier) - xOffset), ((530 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((277 * resolutionXMultiplier) - xOffset), ((530 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((277 * resolutionXMultiplier) - xOffset), ((529 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((277 * resolutionXMultiplier) - xOffset), ((528 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((277 * resolutionXMultiplier) - xOffset), ((527 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((276 * resolutionXMultiplier) - xOffset), ((527 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((276 * resolutionXMultiplier) - xOffset), ((526 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((276 * resolutionXMultiplier) - xOffset), ((525 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((276 * resolutionXMultiplier) - xOffset), ((524 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((276 * resolutionXMultiplier) - xOffset), ((523 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((276 * resolutionXMultiplier) - xOffset), ((522 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((275 * resolutionXMultiplier) - xOffset), ((522 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((275 * resolutionXMultiplier) - xOffset), ((521 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((275 * resolutionXMultiplier) - xOffset), ((520 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((275 * resolutionXMultiplier) - xOffset), ((519 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((275 * resolutionXMultiplier) - xOffset), ((518 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((275 * resolutionXMultiplier) - xOffset), ((516 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((275 * resolutionXMultiplier) - xOffset), ((515 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((275 * resolutionXMultiplier) - xOffset), ((514 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((275 * resolutionXMultiplier) - xOffset), ((513 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((275 * resolutionXMultiplier) - xOffset), ((512 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((275 * resolutionXMultiplier) - xOffset), ((511 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((275 * resolutionXMultiplier) - xOffset), ((510 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((510 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((509 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((508 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((507 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((506 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((505 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((504 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((503 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((502 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((502 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((501 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((500 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((499 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((498 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((497 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((496 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((495 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((494 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((493 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((492 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((491 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((490 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((489 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((489 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((488 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((487 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((486 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((485 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((484 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((483 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((482 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((481 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((480 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((479 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((478 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((477 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((476 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((475 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((474 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((473 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((472 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((274 * resolutionXMultiplier) - xOffset), ((471 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((471 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((470 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((469 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((468 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((467 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((466 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((273 * resolutionXMultiplier) - xOffset), ((465 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((465 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((464 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((463 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((462 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((461 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((460 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((459 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((458 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((457 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((456 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((456 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((455 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((454 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((453 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((452 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((451 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((450 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((449 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((448 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((447 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((446 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((445 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((444 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((443 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((442 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((441 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((440 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((439 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((438 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((437 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((436 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((435 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((434 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((433 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((432 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((431 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((272 * resolutionXMultiplier) - xOffset), ((430 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((430 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((429 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((428 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((427 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((271 * resolutionXMultiplier) - xOffset), ((426 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((270 * resolutionXMultiplier) - xOffset), ((426 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((270 * resolutionXMultiplier) - xOffset), ((425 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((270 * resolutionXMultiplier) - xOffset), ((424 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((269 * resolutionXMultiplier) - xOffset), ((424 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((269 * resolutionXMultiplier) - xOffset), ((423 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((269 * resolutionXMultiplier) - xOffset), ((422 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((268 * resolutionXMultiplier) - xOffset), ((422 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((268 * resolutionXMultiplier) - xOffset), ((421 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((268 * resolutionXMultiplier) - xOffset), ((420 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((268 * resolutionXMultiplier) - xOffset), ((419 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((267 * resolutionXMultiplier) - xOffset), ((419 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((267 * resolutionXMultiplier) - xOffset), ((418 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((267 * resolutionXMultiplier) - xOffset), ((417 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((267 * resolutionXMultiplier) - xOffset), ((416 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((266 * resolutionXMultiplier) - xOffset), ((416 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((266 * resolutionXMultiplier) - xOffset), ((415 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((266 * resolutionXMultiplier) - xOffset), ((414 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((265 * resolutionXMultiplier) - xOffset), ((414 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((265 * resolutionXMultiplier) - xOffset), ((413 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((265 * resolutionXMultiplier) - xOffset), ((412 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((264 * resolutionXMultiplier) - xOffset), ((412 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((264 * resolutionXMultiplier) - xOffset), ((411 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((264 * resolutionXMultiplier) - xOffset), ((410 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((263 * resolutionXMultiplier) - xOffset), ((410 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((263 * resolutionXMultiplier) - xOffset), ((409 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((262 * resolutionXMultiplier) - xOffset), ((409 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((262 * resolutionXMultiplier) - xOffset), ((408 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((261 * resolutionXMultiplier) - xOffset), ((408 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((260 * resolutionXMultiplier) - xOffset), ((408 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((259 * resolutionXMultiplier) - xOffset), ((408 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((259 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((258 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((257 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((256 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((255 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((254 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((253 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((252 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((251 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((251 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((250 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((249 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((248 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((247 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((246 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((245 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((244 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((243 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((242 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((241 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((240 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((239 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((238 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((237 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((236 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((235 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((234 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((233 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((232 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((231 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((230 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((229 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((228 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((227 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((226 * resolutionXMultiplier) - xOffset), ((406 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((225 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((224 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((223 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((222 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((221 * resolutionXMultiplier) - xOffset), ((407 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((221 * resolutionXMultiplier) - xOffset), ((408 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((220 * resolutionXMultiplier) - xOffset), ((408 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((219 * resolutionXMultiplier) - xOffset), ((408 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((218 * resolutionXMultiplier) - xOffset), ((408 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((218 * resolutionXMultiplier) - xOffset), ((409 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((217 * resolutionXMultiplier) - xOffset), ((409 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((216 * resolutionXMultiplier) - xOffset), ((409 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((216 * resolutionXMultiplier) - xOffset), ((410 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((215 * resolutionXMultiplier) - xOffset), ((410 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((214 * resolutionXMultiplier) - xOffset), ((410 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((214 * resolutionXMultiplier) - xOffset), ((411 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((213 * resolutionXMultiplier) - xOffset), ((411 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((213 * resolutionXMultiplier) - xOffset), ((412 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((213 * resolutionXMultiplier) - xOffset), ((413 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((213 * resolutionXMultiplier) - xOffset), ((414 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((212 * resolutionXMultiplier) - xOffset), ((414 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((212 * resolutionXMultiplier) - xOffset), ((415 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((212 * resolutionXMultiplier) - xOffset), ((416 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((211 * resolutionXMultiplier) - xOffset), ((416 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((211 * resolutionXMultiplier) - xOffset), ((417 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((211 * resolutionXMultiplier) - xOffset), ((418 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((211 * resolutionXMultiplier) - xOffset), ((419 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((211 * resolutionXMultiplier) - xOffset), ((420 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((210 * resolutionXMultiplier) - xOffset), ((420 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((210 * resolutionXMultiplier) - xOffset), ((421 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((210 * resolutionXMultiplier) - xOffset), ((422 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((210 * resolutionXMultiplier) - xOffset), ((423 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((210 * resolutionXMultiplier) - xOffset), ((424 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((210 * resolutionXMultiplier) - xOffset), ((425 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((210 * resolutionXMultiplier) - xOffset), ((426 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((210 * resolutionXMultiplier) - xOffset), ((427 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((210 * resolutionXMultiplier) - xOffset), ((428 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((209 * resolutionXMultiplier) - xOffset), ((428 * resolutionXMultiplier) - yOffset)));
            fusePoints.Add(new Point(((209 * resolutionXMultiplier) - xOffset), ((429 * resolutionXMultiplier) - yOffset)));

        }

        List<Rect> flamingRects = new List<Rect>(2);

        List<List<List<ImageBrush>>> rampageMonsterAnimationFrames = new List<List<List<ImageBrush>>>(2);

        List<ImageBrush> versusBgBrush = new List<ImageBrush>(2);

        List<ImageBrush> rampageBlood = new List<ImageBrush>(4);

        Rect nullRect = new Rect();

        private void ProvisionRampageMonsters()
        {
            ProvisionRampageClimbingAnimation();
            
            // load rampage blood
            for (int rampageBloodIndex = 0; rampageBloodIndex < 4; rampageBloodIndex++)
            {
                ImageBrush rampageBloodBrush = new ImageBrush();

                rampageBloodBrush.ImageSource = BigBlue.ImageLoading.loadImage("bloodsplat" + rampageBloodIndex.ToString() + ".png", integerMultiplier);
                rampageBloodBrush.AlignmentX = AlignmentX.Left;
                rampageBloodBrush.AlignmentY = AlignmentY.Top;
                rampageBloodBrush.TileMode = TileMode.None;
                RenderOptions.SetBitmapScalingMode(rampageBloodBrush, BitmapScalingMode.NearestNeighbor);
                rampageBloodBrush.Freeze();
                rampageBlood.Add(rampageBloodBrush);
            }
            
            // george images
            BitmapImage[] georgeImages = new BitmapImage[4];

            georgeImages[0] = BigBlue.ImageLoading.loadImage("george.png", integerMultiplier);
            georgeImages[1] = BigBlue.ImageLoading.loadImage("george_sunset.png", integerMultiplier);
            georgeImages[2] = BigBlue.ImageLoading.loadImage("george_night.png", integerMultiplier);
            georgeImages[3] = BigBlue.ImageLoading.loadImage("george_dawn.png", integerMultiplier);

            // lizzie images
            BitmapImage[] lizzieImages = new BitmapImage[4];
            lizzieImages[0] = BigBlue.ImageLoading.loadImage("lizzie.png", integerMultiplier);
            lizzieImages[1] = BigBlue.ImageLoading.loadImage("lizzie_sunset.png", integerMultiplier);
            lizzieImages[2] = BigBlue.ImageLoading.loadImage("lizzie_night.png", integerMultiplier);
            lizzieImages[3] = BigBlue.ImageLoading.loadImage("lizzie_dawn.png", integerMultiplier);

            // ralph images
            BitmapImage[] ralphImages = new BitmapImage[4];
            ralphImages[0] = BigBlue.ImageLoading.loadImage("ralph.png", integerMultiplier);
            ralphImages[1] = BigBlue.ImageLoading.loadImage("ralph_sunset.png", integerMultiplier);
            ralphImages[2] = BigBlue.ImageLoading.loadImage("ralph_night.png", integerMultiplier);
            ralphImages[3] = BigBlue.ImageLoading.loadImage("ralph_dawn.png", integerMultiplier);

            List<Rect> rampageRects = new List<Rect>(11);

            flamingRects.Add(new Rect(0, 0, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // flame 1
            flamingRects.Add(new Rect(0, 292, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // flame 2

            rampageRects.Add(new Rect(0, 0, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // frame #0 -- climbing up with foot parallel to building
            rampageRects.Add(new Rect(308, 0, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // frame #1 -- climbing up with foot perpendicular to building
            rampageRects.Add(new Rect(616, 0, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // frame #2 -- climbing down with foot perpendicular to building
            rampageRects.Add(new Rect(924, 0, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // frame #3 -- climbing down with foot paralle to building
            rampageRects.Add(new Rect(308, 292, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT));  // frame #4 -- punching left
            rampageRects.Add(new Rect(616, 292, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // frame #5 -- punching right
            rampageRects.Add(new Rect(0, 292, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // frame #6 -- punching downward
            rampageRects.Add(new Rect(924, 292, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // frame #7 -- spectating (healthy)
            rampageRects.Add(new Rect(0, 584, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // frame #8 -- spectating (dying)
            rampageRects.Add(new Rect(308, 584, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // frame #9 -- lost grip
            rampageRects.Add(new Rect(616, 584, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // frame #10 -- falling
            rampageRects.Add(new Rect(924, 584, RAMPAGE_MONSTER_WIDTH, RAMPAGE_MONSTER_HEIGHT)); // frame #11 -- celebrating


            // for each monster, we need to create a new list of a list of animation frames
            for (int monsterIndex = 0; monsterIndex < 3; monsterIndex++)
            {
                // this is the list of animation frames contained by a list of times contained by a list of monsters
                List<List<ImageBrush>> monsterTimeCollection = new List<List<ImageBrush>>(2);

                // for each of the three times, there's going to be a list of 11 frames of animation
                for (int timeIndex = 0; timeIndex < 4; timeIndex++)
                {
                    List<ImageBrush> monsterImageBrushes = new List<ImageBrush>(11);

                    // for each frame of animation, we need to add it to the list
                    foreach (Rect r in rampageRects)
                    {
                        switch (monsterIndex)
                        {
                            case 0:
                                monsterImageBrushes.Add(BigBlue.ImageLoading.loadAnimationFrame(georgeImages[timeIndex], r, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                                break;
                            case 1:
                                monsterImageBrushes.Add(BigBlue.ImageLoading.loadAnimationFrame(lizzieImages[timeIndex], r, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                                break;
                            case 2:
                                monsterImageBrushes.Add(BigBlue.ImageLoading.loadAnimationFrame(ralphImages[timeIndex], r, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                                break;
                        }
                    }

                    // add the list of image brushes
                    monsterTimeCollection.Add(monsterImageBrushes);
                }

                // add the list of times per monster
                rampageMonsterAnimationFrames.Add(monsterTimeCollection);
            }

            ProvisionRampageBurningAnimation();
        }
        
        private void ProvisionNumbers()
        {
            for (int numberIndex = 0; numberIndex < 10; numberIndex++)
            {
                string numberKey = numberIndex.ToString() + "0";
                string altNumberKey = numberIndex.ToString() + "1";

                ImageBrush numberBrush = new ImageBrush();

                numberBrush.ImageSource = BigBlue.ImageLoading.loadImage(numberKey + ".png", integerMultiplier);
                numberBrush.AlignmentX = AlignmentX.Left;
                numberBrush.AlignmentY = AlignmentY.Top;
                numberBrush.TileMode = TileMode.None;
                RenderOptions.SetBitmapScalingMode(numberBrush, BitmapScalingMode.NearestNeighbor);
                numberBrush.Freeze();

                numbers[numberKey] = numberBrush;

                ImageBrush altNumberBrush = new ImageBrush();

                altNumberBrush.ImageSource = BigBlue.ImageLoading.loadImage(altNumberKey + ".png", integerMultiplier);
                altNumberBrush.AlignmentX = AlignmentX.Left;
                altNumberBrush.AlignmentY = AlignmentY.Top;
                altNumberBrush.TileMode = TileMode.None;
                RenderOptions.SetBitmapScalingMode(altNumberBrush, BitmapScalingMode.NearestNeighbor);
                altNumberBrush.Freeze();

                numbers[altNumberKey] = altNumberBrush;
            }

            // default to 99
            FirstDigit.Fill = numbers["90"];
            SecondDigit.Fill = numbers["90"];

            if (showClock == true)
            {
                ImageBrush colonBrush = new ImageBrush();

                colonBrush.ImageSource = BigBlue.ImageLoading.loadImage("colon.png", integerMultiplier);
                colonBrush.AlignmentX = AlignmentX.Left;
                colonBrush.AlignmentY = AlignmentY.Top;
                colonBrush.TileMode = TileMode.None;
                RenderOptions.SetBitmapScalingMode(colonBrush, BitmapScalingMode.NearestNeighbor);
                colonBrush.Freeze();

                ColonRectangle.Fill = colonBrush;

                ImageBrush anteMeridiemBrush = new ImageBrush();
                
                anteMeridiemBrush.ImageSource = BigBlue.ImageLoading.loadImage("a.png", integerMultiplier);
                anteMeridiemBrush.AlignmentX = AlignmentX.Left;
                anteMeridiemBrush.AlignmentY = AlignmentY.Top;
                anteMeridiemBrush.TileMode = TileMode.None;
                RenderOptions.SetBitmapScalingMode(anteMeridiemBrush, BitmapScalingMode.NearestNeighbor);
                anteMeridiemBrush.Freeze();

                numbers["AM"] = anteMeridiemBrush;

                ImageBrush postMeridiemBrush = new ImageBrush();

                postMeridiemBrush.ImageSource = BigBlue.ImageLoading.loadImage("p.png", integerMultiplier);
                postMeridiemBrush.AlignmentX = AlignmentX.Left;
                postMeridiemBrush.AlignmentY = AlignmentY.Top;
                postMeridiemBrush.TileMode = TileMode.None;
                RenderOptions.SetBitmapScalingMode(postMeridiemBrush, BitmapScalingMode.NearestNeighbor);
                postMeridiemBrush.Freeze();

                numbers["PM"] = postMeridiemBrush;

                ImageBrush meridiemBrush = new ImageBrush();

                meridiemBrush.ImageSource = BigBlue.ImageLoading.loadImage("m.png", integerMultiplier);
                meridiemBrush.AlignmentX = AlignmentX.Left;
                meridiemBrush.AlignmentY = AlignmentY.Top;
                meridiemBrush.TileMode = TileMode.None;
                RenderOptions.SetBitmapScalingMode(meridiemBrush, BitmapScalingMode.NearestNeighbor);
                meridiemBrush.Freeze();

                TwelveHourTwoRectangle.Fill = meridiemBrush;

                // numbers are 48x68
                // a and p are 52px wide
                // m is 56px wide
                // colon is 26px wide
                // margin between numbers is 12px

                int numberWidth = 48;

                double clockCanvasWidth = (numberWidth + 12 + numberWidth + 26 + numberWidth + 12 + numberWidth + 12 + 52 + 56) * resolutionXMultiplier;

                ClockCanvas.Width = Math.Ceiling(clockCanvasWidth);

                ClockCanvas.Height = Math.Ceiling(68 * resolutionXMultiplier);

                HourOneRectangle.Width = numberWidth * resolutionXMultiplier;
                HourOneRectangle.Height = 68 * resolutionXMultiplier;

                Canvas.SetLeft(HourOneRectangle, 0);
                Canvas.SetTop(HourOneRectangle, 0);

                HourTwoRectangle.Width = numberWidth * resolutionXMultiplier;
                HourTwoRectangle.Height = 68 * resolutionXMultiplier;

                Canvas.SetTop(HourTwoRectangle, 0);
                Canvas.SetLeft(HourTwoRectangle, 60 * resolutionXMultiplier);

                ColonRectangle.Width = 26 * resolutionXMultiplier;
                ColonRectangle.Height = 68 * resolutionXMultiplier;

                Canvas.SetTop(ColonRectangle, 0);
                Canvas.SetLeft(ColonRectangle, 108 * resolutionXMultiplier);

                MinuteOneRectangle.Width = numberWidth * resolutionXMultiplier;
                MinuteOneRectangle.Height = 68 * resolutionXMultiplier;

                Canvas.SetLeft(MinuteOneRectangle, 134 * resolutionXMultiplier);
                Canvas.SetTop(MinuteOneRectangle, 0);

                MinuteTwoRectangle.Width = numberWidth * resolutionXMultiplier;
                MinuteTwoRectangle.Height = 68 * resolutionXMultiplier;

                Canvas.SetLeft(MinuteTwoRectangle, 194 * resolutionXMultiplier);
                Canvas.SetTop(MinuteTwoRectangle, 0);

                TwelveHourOneRectangle.Width = 52 * resolutionXMultiplier;
                TwelveHourOneRectangle.Height = 68 * resolutionXMultiplier;

                Canvas.SetLeft(TwelveHourOneRectangle, 254 * resolutionXMultiplier);
                Canvas.SetTop(TwelveHourOneRectangle, 0);

                TwelveHourTwoRectangle.Width = 56 * resolutionXMultiplier;
                TwelveHourTwoRectangle.Height = 68 * resolutionXMultiplier;

                Canvas.SetLeft(TwelveHourTwoRectangle, 306 * resolutionXMultiplier);
                Canvas.SetTop(TwelveHourTwoRectangle, 0);

                UpdateClock();
            }
            else
            {
                ClockCanvas.Visibility = Visibility.Collapsed;
            }
        }

        List<List<ImageBrush>> rtypeAnimationFrames = new List<List<ImageBrush>>(3);

        ImageBrush rtypeLaserBrush;

        private void ProvisionRtype()
        {
            ProvisionRtypeFlyingAnimation();

            BitmapImage rtypeLaserImage = BigBlue.ImageLoading.loadImage("rtypelaser.png", integerMultiplier);

            rtypeLaserBrush = new ImageBrush();
            rtypeLaserBrush.ImageSource = rtypeLaserImage;
            rtypeLaserBrush.TileMode = TileMode.None;
            rtypeLaserBrush.AlignmentX = AlignmentX.Left;
            rtypeLaserBrush.AlignmentY = AlignmentY.Top;
            rtypeLaserBrush.Freeze();

            BitmapImage rtypeImageDay = BigBlue.ImageLoading.loadImage("rtypeship.png", integerMultiplier);
            BitmapImage rtypeImageSunset = BigBlue.ImageLoading.loadImage("rtype_sunset.png", integerMultiplier);
            BitmapImage rtypeImageNight = BigBlue.ImageLoading.loadImage("rtype_night.png", integerMultiplier);
            BitmapImage rtypeImageDawn = BigBlue.ImageLoading.loadImage("rtype_dawn.png", integerMultiplier);

            for (int i = 0; i < 4; i++)
            {
                List<ImageBrush> rtypeImageBrushes = new List<ImageBrush>(11);
                rtypeAnimationFrames.Add(rtypeImageBrushes);
            }


            Rect[] rtypeRects = new Rect[11];
            rtypeRects[0] = new Rect(0, 0, 154, 118); // frame #1
            rtypeRects[1] = new Rect(158, 0, 154, 118); // frame #2
            rtypeRects[2] = new Rect(316, 0, 154, 118); // frame #3
            rtypeRects[3] = new Rect(474, 0, 154, 118); // frame #4
            rtypeRects[4] = new Rect(632, 0, 154, 118); // frame #5
            rtypeRects[5] = new Rect(790, 0, 154, 118); // frame #6
            rtypeRects[6] = new Rect(948, 0, 154, 118); // frame #7
            rtypeRects[7] = new Rect(1106, 0, 154, 118); // frame #8
            rtypeRects[8] = new Rect(1264, 0, 154, 118); // frame #9
            rtypeRects[9] = new Rect(1422, 0, 154, 118); // frame #10
            rtypeRects[10] = new Rect(1580, 0, 154, 118); // frame #11


            foreach (Rect r in rtypeRects)
            {
                rtypeAnimationFrames[0].Add(BigBlue.ImageLoading.loadAnimationFrame(rtypeImageDay, r, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                rtypeAnimationFrames[1].Add(BigBlue.ImageLoading.loadAnimationFrame(rtypeImageSunset, r, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                rtypeAnimationFrames[2].Add(BigBlue.ImageLoading.loadAnimationFrame(rtypeImageNight, r, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                rtypeAnimationFrames[3].Add(BigBlue.ImageLoading.loadAnimationFrame(rtypeImageDawn, r, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
            }

        }
        
        private void ChooseRampageCharacter(bool random)
        {
            try
            {
                if (random == true)
                {
                    //rampageCharacterChoice = r.Next(0, 3);
                    if (rampageCharacterChoice == 2)
                    {
                        rampageCharacterChoice = 0;
                    }
                    else
                    {
                        rampageCharacterChoice = rampageCharacterChoice + 1;
                    }
                }

                if (freeForAll == false)
                {
                    VersusPortraits.Fill = versusBgBrush[rampageCharacterChoice];
                    if (surroundPosition != BigBlue.Models.SurroundPosition.None)
                    {
                        VersusPortraitsSecondary.Fill = versusBgBrush[rampageCharacterChoice];
                    }
                    
                    OneNameImage.Viewbox = lifeBarNameRects[rampageCharacterChoice];
                }
            }
            catch (Exception)
            {
            }
        }

        private void SetBushTileGraphics()
        {
            foreach (Rectangle bushRec in bushTiles)
            {
                bushRec.Fill = bushImageBrushes[timeCode];
            }
        }

        private void SetSpectatorTileGraphics()
        {
            foreach (Rectangle spectatorTile in spectatorTiles)
            {
                spectatorTile.Fill = spectatorAnimations[streetFighterEditionAnimation][timeCode][spectatorsCurrentFrame];
            }
        }

        private void SetTimeDependentGraphics(bool transition)
        {
            Rtype.Fill = rtypeAnimationFrames[timeCode][rtypeCurrentFrame];
            FightersRectangle.Fill = fightersAnimationFrames[timeCode][fighterCurrentFrame];
            RampageMonsterRectangle.Fill = rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][rampageCurrentFrame];
            GeorgeDamageDecal.Fill = rampageBlood[timeCode];
            BgBuildingsRectangle.Fill = bgBuildings[timeCode];

            SetBushTileGraphics();

            BuildingRectangle.Fill = mainBuildingImages[timeCode][portraitModeIndex];

            SetSpectatorTileGraphics();

            ScreenBackgroundRectangle.Fill = buildingSnapshotFrameImages[timeCode][portraitModeIndex];
            BuildingRoofRectangle.Fill = buildingRoofImages[timeCode][portraitModeIndex];

            ColorBuildingObjects();
        }

        private int CalculateCurrentTime()
        {
            int currentTimeCode = -1;

            DateTime currentTime = DateTime.Now;
            
            int hour = currentTime.Hour;

            if (hour >= 17 && hour < 19)
            {
                currentTimeCode = 1;
            }
            else if ((hour >= 19 && hour < 24) || (hour >= 0 && hour < 4))
            {
                currentTimeCode = 2;
            }
            else if (hour >= 4 && hour < 6)
            {
                currentTimeCode = 3;
            }
            else
            {
                currentTimeCode = 0;
            }

            return currentTimeCode;
        }

        private bool CalculateTimeCode()
        {
            int currentTimeCode = CalculateCurrentTime();

            // if the time code hasn't changed, we're not going to bother redoing all the graphics
            if (currentTimeCode == timeCode)
            {
                return false;
            }
            else
            {
                timeCode = currentTimeCode;
                return true;
            }
        }

        private void CalculateTime(bool updateGameList)
        {
            if (dynamicTimeOfDay == true)
            {
                if (CalculateTimeCode() == true)
                {
                    RenderTimeBasedOnCalculation(false);

                    if (updateGameList == true)
                    {
                        RenderGameList(false);
                    }
                }
            }
        }

        private void InstantMorning()
        {
            Stippled1MorningStoryBoard.Begin();
            Stippled1MorningStoryBoard.SkipToFill();
            Stippled2MorningStoryBoard.Begin();
            Stippled2MorningStoryBoard.SkipToFill();
            Stippled3MorningStoryBoard.Begin();
            Stippled3MorningStoryBoard.SkipToFill();
            Stippled4MorningStoryBoard.Begin();
            Stippled4MorningStoryBoard.SkipToFill();

            SkyGradientMorningStoryBoard1.Begin();
            SkyGradientMorningStoryBoard1.SkipToFill();
            SkyGradientMorningStoryBoard2.Begin();
            SkyGradientMorningStoryBoard2.SkipToFill();
            SkyGradientMorningStoryBoard3.Begin();
            SkyGradientMorningStoryBoard3.SkipToFill();
            SkyGradientMorningStoryBoard4.Begin();
            SkyGradientMorningStoryBoard4.SkipToFill();
            SkyGradientMorningStoryBoard5.Begin();
            SkyGradientMorningStoryBoard5.SkipToFill();
        }

        private void InstantSunset()
        {
            Stippled1SunsetStoryBoard.Begin();
            Stippled1SunsetStoryBoard.SkipToFill();
            Stippled2SunsetStoryBoard.Begin();
            Stippled2SunsetStoryBoard.SkipToFill();
            Stippled3SunsetStoryBoard.Begin();
            Stippled3SunsetStoryBoard.SkipToFill();
            Stippled4SunsetStoryBoard.Begin();
            Stippled4SunsetStoryBoard.SkipToFill();

            SkyGradientSunsetStoryBoard1.Begin();
            SkyGradientSunsetStoryBoard1.SkipToFill();
            SkyGradientSunsetStoryBoard2.Begin();
            SkyGradientSunsetStoryBoard2.SkipToFill();
            SkyGradientSunsetStoryBoard3.Begin();
            SkyGradientSunsetStoryBoard3.SkipToFill();
            SkyGradientSunsetStoryBoard4.Begin();
            SkyGradientSunsetStoryBoard4.SkipToFill();
            SkyGradientSunsetStoryBoard5.Begin();
            SkyGradientSunsetStoryBoard5.SkipToFill();
        }

        private void InstantNight()
        {
            Stippled1NightStoryBoard.Begin();
            Stippled1NightStoryBoard.SkipToFill();
            Stippled2NightStoryBoard.Begin();
            Stippled2NightStoryBoard.SkipToFill();
            Stippled3NightStoryBoard.Begin();
            Stippled3NightStoryBoard.SkipToFill();
            Stippled4NightStoryBoard.Begin();
            Stippled4NightStoryBoard.SkipToFill();

            SkyGradientNightStoryBoard1.Begin();
            SkyGradientNightStoryBoard1.SkipToFill();
            SkyGradientNightStoryBoard2.Begin();
            SkyGradientNightStoryBoard2.SkipToFill();
            SkyGradientNightStoryBoard3.Begin();
            SkyGradientNightStoryBoard3.SkipToFill();
            SkyGradientNightStoryBoard4.Begin();
            SkyGradientNightStoryBoard4.SkipToFill();
            SkyGradientNightStoryBoard5.Begin();
            SkyGradientNightStoryBoard5.SkipToFill();
        }

        private void RenderTimeBasedOnCalculation(bool transition)
        {
            Blood.Fill = fighterBloodBrushes[timeCode];
            Blood2.Fill = fighterBloodBrushes[timeCode];
            Blood3.Fill = fighterBloodBrushes[timeCode];
            Blood4.Fill = fighterBloodBrushes[timeCode];

            switch (timeCode)
            {
                case 0:
                    if (transition == false)
                    {
                        InstantMorning();
                    }

                    Cloud3Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud4Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud5Rectangle.Visibility = System.Windows.Visibility.Collapsed;

                    Cloud1Rectangle.Visibility = System.Windows.Visibility.Visible;
                    Cloud2Rectangle.Visibility = System.Windows.Visibility.Visible;

                    if (DependencyPropertyHelper.GetValueSource(Cloud3Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud3StoryBoard.Stop();
                    }

                    if (DependencyPropertyHelper.GetValueSource(Cloud4Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud4StoryBoard.Stop();
                    }

                    if (DependencyPropertyHelper.GetValueSource(Cloud5Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud5StoryBoard.Stop();
                    }

                    Cloud1StoryBoard.Begin();
                    Cloud2StoryBoard.Begin();

                    SetTimeDependentGraphics(transition);

                    break;
                case 1:
                    if (transition == false)
                    {
                        InstantSunset();
                    }

                    if (DependencyPropertyHelper.GetValueSource(Cloud1Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud1StoryBoard.Stop();
                    }
                    if (DependencyPropertyHelper.GetValueSource(Cloud2Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud2StoryBoard.Stop();
                    }
                    if (DependencyPropertyHelper.GetValueSource(Cloud4Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud4StoryBoard.Stop();
                    }

                    if (DependencyPropertyHelper.GetValueSource(Cloud5Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud5StoryBoard.Stop();
                    }

                    // hide the other cloud layers
                    Cloud1Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud2Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud4Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud5Rectangle.Visibility = System.Windows.Visibility.Collapsed;

                    // show the sunset cloud layer
                    Cloud3Rectangle.Visibility = System.Windows.Visibility.Visible;

                    Cloud3StoryBoard.Begin();

                    SetTimeDependentGraphics(transition);

                    break;
                case 2:
                    if (transition == false)
                    {
                        InstantNight();
                    }
                    
                    if (DependencyPropertyHelper.GetValueSource(Cloud1Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud1StoryBoard.Stop();
                    }

                    if (DependencyPropertyHelper.GetValueSource(Cloud2Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud2StoryBoard.Stop();
                    }

                    if (DependencyPropertyHelper.GetValueSource(Cloud3Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud3StoryBoard.Stop();
                    }

                    if (DependencyPropertyHelper.GetValueSource(Cloud5Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud5StoryBoard.Stop();
                    }

                    Cloud1Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud2Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud3Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud5Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud4Rectangle.Visibility = System.Windows.Visibility.Visible;

                    Cloud4StoryBoard.Begin();

                    SetTimeDependentGraphics(transition);
                    break;
                case 3:
                    if (DependencyPropertyHelper.GetValueSource(Cloud1Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud1StoryBoard.Stop();
                    }

                    if (DependencyPropertyHelper.GetValueSource(Cloud2Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud2StoryBoard.Stop();
                    }

                    if (DependencyPropertyHelper.GetValueSource(Cloud3Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud3StoryBoard.Stop();
                    }

                    if (DependencyPropertyHelper.GetValueSource(Cloud4Rectangle, Canvas.LeftProperty).IsAnimated)
                    {
                        Cloud4StoryBoard.Stop();
                    }

                    Cloud1Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud2Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud3Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud4Rectangle.Visibility = System.Windows.Visibility.Collapsed;
                    Cloud5Rectangle.Visibility = System.Windows.Visibility.Visible;
                    
                    Cloud5StoryBoard.Begin();
                    SetTimeDependentGraphics(transition);
                    break;
            }

            StyleGameListTextBlocks();
        }

        private SolidColorBrush CreateStippledSkyLayer(Rectangle skyLayer, int colorIndex)
        {
            DrawingBrush db = new DrawingBrush();

            db.Stretch = Stretch.None;
            // weow
            db.TileMode = TileMode.Tile;
            db.Viewport = new Rect(0, 0, 10 * resolutionXMultiplier, 10 * resolutionXMultiplier);
            db.ViewportUnits = BrushMappingMode.Absolute;

            GeometryDrawing skyGeometryDrawing = new GeometryDrawing();
            GeometryGroup skyGeometryGroup = new GeometryGroup();

            RectangleGeometry skyRectangleGeometry = new RectangleGeometry(new Rect(0, 0, 5 * resolutionXMultiplier, 5 * resolutionXMultiplier));
            skyRectangleGeometry.Freeze();
            skyGeometryGroup.Children.Add(skyRectangleGeometry);
            RectangleGeometry skyRectangleGeometry2 = new RectangleGeometry(new Rect(5 * resolutionXMultiplier, 5 * resolutionXMultiplier, 5 * resolutionXMultiplier, 5 * resolutionXMultiplier));
            skyRectangleGeometry2.Freeze();
            skyGeometryGroup.Children.Add(skyRectangleGeometry2);
            skyGeometryGroup.Freeze();

            skyGeometryDrawing.Geometry = skyGeometryGroup;

            SolidColorBrush skyBrush = new SolidColorBrush();
            skyBrush.Color = stippledLayerColors[timeCode][colorIndex];

            skyGeometryDrawing.Brush = skyBrush;
            db.Drawing = skyGeometryDrawing;
            skyLayer.Fill = db;

            return skyBrush;
        }
        
        List<List<Color>> stippledLayerColors = new List<List<Color>>(4);

        private void ProvisionSky()
        {
            if (dynamicTimeOfDay == true)
            {
                timeCode = CalculateCurrentTime();
            }

            List<Color> morningStippledColors = new List<Color>(4);
            morningStippledColors.Add(morningSkyColor2);
            morningStippledColors.Add(morningSkyColor3);
            morningStippledColors.Add(morningSkyColor4);
            morningStippledColors.Add(morningSkyColor5);
            stippledLayerColors.Add(morningStippledColors);

            List<Color> sunsetStippledColors = new List<Color>(4);
            sunsetStippledColors.Add(sunsetSkyColor2);
            sunsetStippledColors.Add(sunsetSkyColor3);
            sunsetStippledColors.Add(sunsetSkyColor4);
            sunsetStippledColors.Add(sunsetSkyColor5);
            stippledLayerColors.Add(sunsetStippledColors);

            List<Color> nightStippledColors = new List<Color>(4);
            nightStippledColors.Add(nightSkyColor2);
            nightStippledColors.Add(nightSkyColor3);
            nightStippledColors.Add(nightSkyColor4);
            nightStippledColors.Add(nightSkyColor5);
            stippledLayerColors.Add(nightStippledColors);

            List<Color> dawnStippledColors = new List<Color>(4);
            dawnStippledColors.Add(dawnSkyColor2);
            dawnStippledColors.Add(dawnSkyColor3);
            dawnStippledColors.Add(dawnSkyColor4);
            dawnStippledColors.Add(dawnSkyColor5);
            stippledLayerColors.Add(dawnStippledColors);

            // set the initial colors of the four stippled layers


            SolidColorBrush b1 = CreateStippledSkyLayer(SkyGradient1, 0);
            SolidColorBrush b2 = CreateStippledSkyLayer(SkyGradient2, 1);
            SolidColorBrush b3 = CreateStippledSkyLayer(SkyGradient3, 2);
            SolidColorBrush b4 = CreateStippledSkyLayer(SkyGradient4, 3);


            switch (timeCode)
            {
                case 0:
                    SkyBackground1Brush.Color = morningSkyColor1;
                    SkyBackground2Brush.Color = morningSkyColor2;
                    SkyBackground3Brush.Color = morningSkyColor3;
                    SkyBackground4Brush.Color = morningSkyColor4;
                    SkyBackground5Brush.Color = morningSkyColor5;
                    break;
                case 1:
                    SkyBackground1Brush.Color = sunsetSkyColor1;
                    SkyBackground2Brush.Color = sunsetSkyColor2;
                    SkyBackground3Brush.Color = sunsetSkyColor3;
                    SkyBackground4Brush.Color = sunsetSkyColor4;
                    SkyBackground5Brush.Color = sunsetSkyColor5;
                    break;
                case 2:
                    SkyBackground1Brush.Color = nightSkyColor1;
                    SkyBackground2Brush.Color = nightSkyColor2;
                    SkyBackground3Brush.Color = nightSkyColor3;
                    SkyBackground4Brush.Color = nightSkyColor4;
                    SkyBackground5Brush.Color = nightSkyColor5;
                    break;
                case 3:
                    SkyBackground1Brush.Color = dawnSkyColor1;
                    SkyBackground2Brush.Color = dawnSkyColor2;
                    SkyBackground3Brush.Color = dawnSkyColor3;
                    SkyBackground4Brush.Color = dawnSkyColor4;
                    SkyBackground5Brush.Color = dawnSkyColor5;
                    break;
            }

            ProvisionMorningAnimation(b1, b2, b3, b4);
            ProvisionSunsetAnimation(b1, b2, b3, b4);
            ProvisionNightAnimation(b1, b2, b3, b4);
            ProvisionDawnAnimation(b1, b2, b3, b4);
        }
        
        Color morningSkyColor1 = Color.FromRgb(91, 188, 227);
        Color morningSkyColor2 = Color.FromRgb(116, 207, 246);
        Color morningSkyColor3 = Color.FromRgb(134, 227, 246);
        Color morningSkyColor4 = Color.FromRgb(170, 227, 246);
        Color morningSkyColor5 = Color.FromRgb(188, 248, 248);

        Color sunsetSkyColor1 = Color.FromRgb(96, 0, 0);
        Color sunsetSkyColor2 = Color.FromRgb(112, 16, 0);
        Color sunsetSkyColor3 = Color.FromRgb(136, 32, 0);
        Color sunsetSkyColor4 = Color.FromRgb(152, 48, 0);
        Color sunsetSkyColor5 = Color.FromRgb(168, 64, 0);

        Color nightSkyColor1 = Color.FromRgb(16, 16, 64);
        Color nightSkyColor2 = Color.FromRgb(16, 16, 80);
        Color nightSkyColor3 = Color.FromRgb(16, 32, 96);
        Color nightSkyColor4 = Color.FromRgb(16, 48, 96);
        Color nightSkyColor5 = Color.FromRgb(32, 64, 96);
        
        Color dawnSkyColor1 = Color.FromRgb(60, 108, 125);
        Color dawnSkyColor2 = Color.FromRgb(60, 108, 142);
        Color dawnSkyColor3 = Color.FromRgb(60, 108, 160);
        Color dawnSkyColor4 = Color.FromRgb(60, 108, 179);
        Color dawnSkyColor5 = Color.FromRgb(60, 125, 198);

        private void SetSkyDimensions()
        {
            bushHeight = 376 * resolutionXMultiplier;

            double doubleScreenWidth = width * 2;

            double skyGradient1Height = 35 * resolutionXMultiplier;
            double skyGradient2Height = 30 * resolutionXMultiplier;
            double skyGradient3Height = 25 * resolutionXMultiplier;
            double skyGradient4Height = 20 * resolutionXMultiplier;

            // going to set the dimensions of the game list and get the margin first; we'll use this to figure out the sky gradient

            bool validGameListDimensions = false;

            while (validGameListDimensions == false)
            {
                validGameListDimensions = SetGameListDimensions(GameList, GameListOutlines, totalBuildingsWidth, bushHeight, null, null);
            }

            // set sky dimensions
            // 395 = 376 (bush height) + 19 (gradient4 height)
            double targetSkyBlocksHeight = height - (bushHeight + skyGradient4Height);

            double fullSkyLayerHeight = Math.Floor((targetSkyBlocksHeight * 2) / 7);
            double halfSkyLayerHeight = fullSkyLayerHeight / 2;

            double skyBackground3VerticalOffset = fullSkyLayerHeight * 2;
            double skyBackground4VerticalOffset = fullSkyLayerHeight * 3;
            double skyBackground5VerticalOffset = skyBackground4VerticalOffset + halfSkyLayerHeight;

            SkyBackground1.Width = width;
            SkyBackground1.Height = fullSkyLayerHeight;

            SkyBackground2.Width = width;
            SkyBackground2.Height = fullSkyLayerHeight;

            SkyBackground3.Width = width;
            SkyBackground3.Height = fullSkyLayerHeight;

            SkyBackground4.Width = width;
            SkyBackground4.Height = halfSkyLayerHeight;

            SkyBackground5.Width = width;
            // this is double the full sky layer height
            SkyBackground5.Height = skyBackground3VerticalOffset;

            Canvas.SetTop(SkyBackground1, 0);
            Canvas.SetTop(SkyBackground2, fullSkyLayerHeight);
            Canvas.SetTop(SkyBackground3, skyBackground3VerticalOffset);
            Canvas.SetTop(SkyBackground4, skyBackground4VerticalOffset);
            Canvas.SetTop(SkyBackground5, skyBackground5VerticalOffset - skyGradient4Height);

            SkyGradient1.Width = doubleScreenWidth;
            SkyGradient1.Height = skyGradient1Height;
            
            SkyGradient2.Width = doubleScreenWidth;
            SkyGradient2.Height = skyGradient2Height;
            
            SkyGradient3.Width = doubleScreenWidth;
            SkyGradient3.Height = skyGradient3Height;
            
            SkyGradient4.Width = doubleScreenWidth;
            SkyGradient4.Height = skyGradient4Height;

            Canvas.SetTop(SkyGradient1, Math.Ceiling(fullSkyLayerHeight - skyGradient1Height));
            Canvas.SetTop(SkyGradient2, Math.Ceiling(skyBackground3VerticalOffset - skyGradient2Height));
            Canvas.SetTop(SkyGradient3, Math.Ceiling(skyBackground4VerticalOffset - skyGradient3Height));
            Canvas.SetTop(SkyGradient4, Math.Ceiling(skyBackground5VerticalOffset - skyGradient4Height));

            // set cloud dimensions
            Cloud1Rectangle.Width = 786 * resolutionXMultiplier;
            Cloud1Rectangle.Height = 254 * resolutionXMultiplier;

            Cloud2Rectangle.Width = 161 * resolutionXMultiplier;
            Cloud2Rectangle.Height = 91 * resolutionXMultiplier;

            double availableSpaceForLargeCloud = skyBackground3VerticalOffset - SkyGradient2.Height;
            double largeCloudSpaceMidPoint = availableSpaceForLargeCloud / 2;
            double largeCloudVerticalOffset = largeCloudSpaceMidPoint - (Cloud1Rectangle.Height / 2);
            
            double availableSpaceForSmallCloud = fullSkyLayerHeight - SkyGradient1.Height;
            double smallCloudSpaceMidPoint = availableSpaceForSmallCloud / 2;
            double smallCloudVerticalOffset = smallCloudSpaceMidPoint - (Cloud2Rectangle.Height / 2);

            Cloud1Rectangle.Margin = new Thickness(0, largeCloudVerticalOffset, 0, 0);

            Cloud2Rectangle.Margin = new Thickness(0, smallCloudVerticalOffset, 0, 0);

            double ryuCloudWidth = 2021 * resolutionXMultiplier;
            double ryuCloudHeight = 402 * resolutionXMultiplier;

            double availableSpaceRyuCloud1 = skyBackground3VerticalOffset - SkyGradient2.Height;
            double ryuCloudSpaceMidPoint1 = availableSpaceRyuCloud1 / 2;
            double ryuCloudVerticalOffset = ryuCloudSpaceMidPoint1 - (ryuCloudHeight / 2);

            if (ryuCloudHeight >= availableSpaceRyuCloud1)
            {
                double availableSpaceRyuCloud2 = skyBackground4VerticalOffset - SkyGradient3.Height;
                double ryuCloudSpaceMidPoint2 = availableSpaceRyuCloud2 / 2;
                ryuCloudVerticalOffset = ryuCloudSpaceMidPoint2 - (ryuCloudHeight / 2);
            }

            Cloud3Rectangle.Width = ryuCloudWidth;
            Cloud3Rectangle.Height = ryuCloudHeight;
            Cloud3Rectangle.Margin = new Thickness(0, ryuCloudVerticalOffset, 0, 0);

            Cloud4Rectangle.Width = ryuCloudWidth;
            Cloud4Rectangle.Height = ryuCloudHeight;
            Cloud4Rectangle.Margin = new Thickness(0, ryuCloudVerticalOffset, 0, 0);

            Cloud5Rectangle.Width = ryuCloudWidth;
            Cloud5Rectangle.Height = ryuCloudHeight;
            Cloud5Rectangle.Margin = new Thickness(0, ryuCloudVerticalOffset, 0, 0);
        }

        
        
        Point fuseCollisionOffset = new Point(0, 0);
        
        private void AnimateFuse()
        {
            fuseElapsedMilliseconds = fuseElapsedMilliseconds + elapsedMilliseconds;

            //if (fuseTimer.ElapsedMilliseconds > 34)
            if (fuseElapsedMilliseconds > 34)
            {
                CalculateFuseCollision();

                double fuseXpos = fusePoints[fuseFrameToDisplay].X;
                double fuseYpos = fusePoints[fuseFrameToDisplay].Y;

                Canvas.SetLeft(IgniteCollisionBox, fuseXpos + fuseCollisionOffset.X);
                Canvas.SetTop(IgniteCollisionBox, fuseYpos + fuseCollisionOffset.Y);

                Canvas.SetLeft(IgniteRectangle, fuseXpos);
                Canvas.SetTop(IgniteRectangle, fuseYpos);

                //fuseTimer.Stop();
                //fuseTimer.Reset();
                //fuseTimer.Start();
                fuseElapsedMilliseconds = 0;

                if ((fuseFrameToDisplay + 1) <= fusePoints.Count - 1)
                {
                    fuseFrameToDisplay = fuseFrameToDisplay + 1;
                }
                else
                {
                    HaggarHeadRectangle.Fill = haggarAnimationFrames[0];
                    //HaggarImage.Viewbox = haggarRects[0];

                    // why would you say he isn't in any danger anymore when he's getting turned into pizza toppings?
                    haggarInDanger = false;
                    BigBlue.XAudio2Player.StopSound("fuse");

                    staticAfterExplosionStoryboard.Begin();
                }
            }
        }
        
        List<List<ImageBrush>> mainBuildingImages = new List<List<ImageBrush>>(3);
        List<List<ImageBrush>> buildingSnapshotFrameImages = new List<List<ImageBrush>>(3);
        List<List<ImageBrush>> buildingRoofImages = new List<List<ImageBrush>>(3);

        private void ProvisionMainBuilding()
        {
            for (int timeIndex = 0; timeIndex < 4; timeIndex++)
            {
                List<ImageBrush> mainBuildingBrushes = new List<ImageBrush>(2);
                List<ImageBrush> buildingSnapshotFrameBrushes = new List<ImageBrush>(2);
                List<ImageBrush> buildingRoofBrushes = new List<ImageBrush>(2);

                for (int aspectIndex = 0; aspectIndex < 2; aspectIndex++)
                {
                    ImageBrush mainBuildingBrush = new ImageBrush();
                    mainBuildingBrush.ImageSource = BigBlue.ImageLoading.loadImage("mainbuilding" + timeIndex.ToString() + aspectIndex.ToString() + ".png", integerMultiplier);
                    mainBuildingBrush.TileMode = TileMode.Tile;
                    mainBuildingBrush.Stretch = Stretch.Uniform;
                    mainBuildingBrush.ViewportUnits = BrushMappingMode.Absolute;

                    if (aspectIndex == 0)
                    {
                        mainBuildingBrush.Viewport = new Rect(0, 0, 960 * resolutionXMultiplier, 232 * resolutionXMultiplier);
                    }
                    else
                    {
                        mainBuildingBrush.Viewport = new Rect(0, 0, 818 * resolutionXMultiplier, 232 * resolutionXMultiplier);
                    }

                    RenderOptions.SetBitmapScalingMode(mainBuildingBrush, BitmapScalingMode.NearestNeighbor);

                    mainBuildingBrush.Freeze();
                    mainBuildingBrushes.Add(mainBuildingBrush);

                    // do the same thing for frame 
                    ImageBrush buildingFrameBrush = new ImageBrush();
                    
                    if (aspectRatioIndex == 2 && aspectIndex == 0)
                    {
                        buildingFrameBrush.ImageSource = BigBlue.ImageLoading.loadImage("screenbg" + timeIndex.ToString() + "2.png", integerMultiplier);
                    }
                    else
                    {
                        buildingFrameBrush.ImageSource = BigBlue.ImageLoading.loadImage("screenbg" + timeIndex.ToString() + aspectIndex.ToString() + ".png", integerMultiplier);
                    }
                
                    buildingFrameBrush.TileMode = TileMode.None;
                    RenderOptions.SetBitmapScalingMode(buildingFrameBrush, BitmapScalingMode.NearestNeighbor);
                    buildingFrameBrush.Freeze();
                    buildingSnapshotFrameBrushes.Add(buildingFrameBrush);

                    // do the same thing for roof
                    ImageBrush buildingRoofBrush = new ImageBrush();
                    buildingRoofBrush.ImageSource = BigBlue.ImageLoading.loadImage("mainbuildingroof" + timeIndex.ToString() + aspectIndex.ToString() + ".png", integerMultiplier);
                    buildingRoofBrush.TileMode = TileMode.None;
                    RenderOptions.SetBitmapScalingMode(buildingRoofBrush, BitmapScalingMode.NearestNeighbor);
                    buildingRoofBrush.Freeze();
                    buildingRoofBrushes.Add(buildingRoofBrush);
                }

                mainBuildingImages.Add(mainBuildingBrushes);
                buildingSnapshotFrameImages.Add(buildingSnapshotFrameBrushes);
                buildingRoofImages.Add(buildingRoofBrushes);
            }
        }

        double screenBackgroundTopPosition = 0;

        private void SetBackgroundDimensions()
        {
            FrontEndContainer.Width = width;
            FrontEndContainer.Height = height;
           
            if (portraitModeIndex == 1)
            {
                BuildingRectangle.Width = Math.Ceiling(818 * resolutionXMultiplier);
            }
            else
            {
                BuildingRectangle.Width = 960 * resolutionXMultiplier;
            }

            chunkHeight = 91 * resolutionXMultiplier;

            BuildingRectangle.Height = height;

            Canvas.SetRight(BuildingRectangle, Math.Ceiling(240 * resolutionXMultiplier));
            Canvas.SetRight(BuildingRoofRectangle, Math.Ceiling(240 * resolutionXMultiplier));

            // frame of the snapshot
            double screenBackgroundWidth = 0;
            double screenBackgroundHeight = 0;

            switch (aspectRatioIndex)
            {
                case 1:
                    screenBackgroundWidth = Math.Floor(475 * resolutionXMultiplier);
                    screenBackgroundHeight = Math.Floor(620 * resolutionXMultiplier);
                    screenBackgroundTopPosition = 163 * resolutionXMultiplier;
                    break;
                case 2:
                    screenBackgroundWidth = Math.Floor(615 * resolutionXMultiplier);
                    screenBackgroundHeight = Math.Floor(374 * resolutionXMultiplier);
                    screenBackgroundTopPosition = 182 * resolutionXMultiplier;
                    break;
                default:
                    screenBackgroundWidth = Math.Floor(615 * resolutionXMultiplier);
                    screenBackgroundHeight = Math.Floor(480 * resolutionXMultiplier);
                    screenBackgroundTopPosition = 159 * resolutionXMultiplier;
                    break;
            }

            double screenBackgroundCorrection = 0; 

            if (!integerMultiplier)
            {
                screenBackgroundCorrection = 2;
            }

            ScreenBackgroundRectangle.Width = screenBackgroundWidth - screenBackgroundCorrection;
            ScreenBackgroundRectangle.Height = screenBackgroundHeight - screenBackgroundCorrection;

            Canvas.SetRight(ScreenBackgroundRectangle, Math.Ceiling(412 * resolutionXMultiplier) + screenBackgroundCorrection);

            double screenBackgroundBottomPosition = screenBackgroundTopPosition + screenBackgroundHeight;

            double snapShotOffset = 0;

            // if we're in free for all mode, we don't need to display the flashing "press start" text, so if we're cramped for vertical space, we're going to raise the snapshot/video area slightly
            if (screenBackgroundBottomPosition > rtypeVerticalPosition && freeForAll == true)
            {
                snapShotOffset = 82 * resolutionXMultiplier;
            }

            Canvas.SetTop(ScreenBackgroundRectangle, (screenBackgroundTopPosition - snapShotOffset));

            // snapshot
            if (portraitModeIndex == 1)
            {
                snapShotWidth = Math.Ceiling(422 * resolutionXMultiplier);
                snapShotHeight = Math.Ceiling(562 * resolutionXMultiplier);

                IgniteRectangle.Width = 78 * resolutionXMultiplier;
                IgniteRectangle.Height = 101 * resolutionXMultiplier;

                IgniteCollisionBox.Width = 28 * resolutionXMultiplier;
                IgniteCollisionBox.Height = 36 * resolutionXMultiplier;

                BuildingRoofRectangle.Width = Math.Ceiling(818 * resolutionXMultiplier);

                double knifeWidth = 62 * resolutionXMultiplier;

                KnifeRectangle.Width = knifeWidth;
                KnifeRectangle.Height = 214 * resolutionXMultiplier;

                knifeCollisionRect.X = 263 * resolutionXMultiplier;
                knifeCollisionRect.Y = KnifeRectangle.Height;

                Canvas.SetLeft(KnifeRectangle, 245 * resolutionXMultiplier);

                knifeCollisionRect.Width = 26 * resolutionXMultiplier;

                Canvas.SetLeft(IgniteRectangle, 275 * resolutionXMultiplier);
                Canvas.SetTop(IgniteRectangle, 485 * resolutionXMultiplier);

                knifeTableOffset = 400 * resolutionXMultiplier;
            }
            else
            {
                // for non 3:4 aspect ratios, the base width for snapshots will always be 562 pixels
                snapShotWidth = Math.Ceiling(562 * resolutionXMultiplier);

                if (aspectRatioIndex == 2)
                {
                    snapShotHeight = Math.Ceiling(316 * resolutionXMultiplier);

                    IgniteRectangle.Width = 65 * resolutionXMultiplier;
                    IgniteRectangle.Height = 85 * resolutionXMultiplier;

                    IgniteCollisionBox.Width = 27 * resolutionXMultiplier;
                    IgniteCollisionBox.Height = 31 * resolutionXMultiplier;

                    KnifeRectangle.Width = 52 * resolutionXMultiplier;
                    KnifeRectangle.Height = 180 * resolutionXMultiplier;

                    knifeCollisionRect.X = 329 * resolutionXMultiplier;
                    knifeCollisionRect.Y = KnifeRectangle.Height;

                    Canvas.SetLeft(KnifeRectangle, 313 * resolutionXMultiplier);

                    knifeCollisionRect.Width = 22 * resolutionXMultiplier;

                    Canvas.SetLeft(IgniteRectangle, 345 * resolutionXMultiplier);
                    Canvas.SetTop(IgniteRectangle, 315 * resolutionXMultiplier);

                    knifeTableOffset = 177 * resolutionXMultiplier;
                }
                else
                {
                    snapShotHeight = Math.Ceiling(422 * resolutionXMultiplier);

                    IgniteRectangle.Width = 88 * resolutionXMultiplier;
                    IgniteRectangle.Height = 113 * resolutionXMultiplier;

                    IgniteCollisionBox.Width = 30 * resolutionXMultiplier;
                    IgniteCollisionBox.Height = 43 * resolutionXMultiplier;

                    KnifeRectangle.Width = 69 * resolutionXMultiplier;
                    KnifeRectangle.Height = 239 * resolutionXMultiplier;

                    knifeCollisionRect.X = 340 * resolutionXMultiplier;
                    knifeCollisionRect.Y = KnifeRectangle.Height;

                    Canvas.SetLeft(KnifeRectangle, 319 * resolutionXMultiplier);

                    knifeCollisionRect.Width = 29 * resolutionXMultiplier;

                    Canvas.SetLeft(IgniteRectangle, 318 * resolutionXMultiplier);
                    Canvas.SetTop(IgniteRectangle, 360 * resolutionXMultiplier);

                    knifeTableOffset = 236 * resolutionXMultiplier;
                }

                BuildingRoofRectangle.Width = 960 * resolutionXMultiplier;
            }

            Canvas.SetTop(KnifeRectangle, -KnifeRectangle.Height);

            fuseCollisionRect.Width = IgniteCollisionBox.Width;
            fuseCollisionRect.Height = IgniteCollisionBox.Height;

            fuseCollisionOffset.X = IgniteCollisionBox.Width;
            fuseCollisionOffset.Y = IgniteCollisionBox.Height;

            knifeCollisionRect.Height = KnifeRectangle.Height;

            if (integerMultiplier == false)
            {
                snapShotWidth = snapShotWidth + 2;
            }

            SnapShotRectangle.Width = snapShotWidth;
            SnapShotRectangle.Height = snapShotHeight;
            SnapShotBackgroundRectangle.Width = snapShotWidth;
            SnapShotBackgroundRectangle.Height = snapShotHeight;

            HaggarCanvas.Width = snapShotWidth;
            HaggarCanvas.Height = snapShotHeight;

            HaggarBackgroundRectangle.Width = snapShotWidth;
            HaggarBackgroundRectangle.Height = snapShotHeight;

            HaggarHeadRectangle.Width = snapShotWidth;
            HaggarHeadRectangle.Height = snapShotHeight;

            HaggarForegroundRectangle.Width = snapShotWidth;
            HaggarForegroundRectangle.Height = snapShotHeight;

            StaticRectangle.Width = snapShotWidth;
            StaticRectangle.Height = snapShotHeight;

            ScreenDoor.Width = snapShotWidth;
            ScreenDoor.Height = snapShotHeight;

            BuildingRoofRectangle.Height = 56 * resolutionXMultiplier;

            Canvas.SetTop(BuildingRoofRectangle, -Math.Floor(BuildingRoofRectangle.Height));

            double snapShotElementTopPosition = 0;

            switch (aspectRatioIndex)
            {
                case 0:
                    snapShotElementTopPosition = Math.Floor((183 * resolutionXMultiplier) - snapShotOffset);
                    break;
                case 1:
                    snapShotElementTopPosition = Math.Floor((187 * resolutionXMultiplier) - snapShotOffset);
                    break;
                case 2:
                    snapShotElementTopPosition = Math.Floor((206 * resolutionXMultiplier) - snapShotOffset);
                    break;
                default:
                    snapShotElementTopPosition = Math.Floor((183 * resolutionXMultiplier) - snapShotOffset);
                    break;
            }
                        
            Canvas.SetTop(SnapShotRectangle, snapShotElementTopPosition);
            Canvas.SetTop(SnapShotBackgroundRectangle, snapShotElementTopPosition);

            double snapShotRightOffset = Math.Floor(439 * resolutionXMultiplier);
            
            Canvas.SetRight(SnapShotRectangle, snapShotRightOffset);
            Canvas.SetRight(SnapShotBackgroundRectangle, snapShotRightOffset);

            // position the video
            Canvas.SetTop(Video, snapShotElementTopPosition);
            Canvas.SetRight(Video, snapShotRightOffset);

            // position the haggar canvas
            Canvas.SetTop(HaggarCanvas, snapShotElementTopPosition);
            Canvas.SetRight(HaggarCanvas, snapShotRightOffset);

            // snapshot screendoor effect
            Canvas.SetTop(ScreenDoor, snapShotElementTopPosition);
            Canvas.SetRight(ScreenDoor, snapShotRightOffset);

            BgBuildingsRectangle.Width = Math.Ceiling(240 * resolutionXMultiplier);
            BgBuildingsRectangle.Height = Math.Ceiling(BASE_FRONTEND_HEIGHT * resolutionXMultiplier);

            //MainBuildingTiledImage.Viewport = new Rect(0, 0, 960 * resolutionXMultiplier, 232 * resolutionXMultiplier);
        }

        private const double RAMPAGE_MONSTER_WIDTH = 304;
        private const double RAMPAGE_MONSTER_HEIGHT = 288;

        private const double RTYPE_SHIP_LENGTH = 116;
        private const double RTYPE_SHIP_AND_PLASMA_COMBINED_LENGTH = 154;

        private const double EXPECTED_WIDTH_TO_RTYPE_RATIO = BASE_FRONTEND_WIDTH / RTYPE_SHIP_LENGTH;
        private const double EXPECTED_HEIGHT_TO_RAMPAGE_MONSTER_RATIO = BASE_FRONTEND_HEIGHT / RAMPAGE_MONSTER_HEIGHT;

        private void SetCharacterDimensions()
        {
            // set rampage monster dimensions
            double monsterWidth = RAMPAGE_MONSTER_WIDTH * resolutionXMultiplier;
            double monsterHeight = RAMPAGE_MONSTER_HEIGHT * resolutionXMultiplier;
            RampageMonsterRectangle.Width = monsterWidth;
            RampageMonsterRectangle.Height = monsterHeight;
            RampageMonsterFlameRectangle.Width = monsterWidth;
            RampageMonsterFlameRectangle.Height = monsterHeight;

            // flip
            //RampageMonsterRectangle.RenderTransformOrigin = new Point(0.5, 0.5);
            //ScaleTransform flipTransform = new ScaleTransform();
            //flipTransform.ScaleX = -1;
            //flipTransform.ScaleY = -1;
            //RampageMonsterRectangle.RenderTransform = flipTransform;

            Canvas.SetZIndex(RampageMonsterRectangle, 50);
            Canvas.SetZIndex(RampageMonsterFlameRectangle, 50);

            Canvas.SetZIndex(DrawGameRectangle, 60);
            Canvas.SetZIndex(DoubleKODecision, 60);
            Canvas.SetZIndex(WinnerRectangle, 60);
            
            georgeBodyCollisionRect.Width = Math.Ceiling(100 * resolutionXMultiplier);
            georgeBodyCollisionRect.Height = Math.Ceiling(180 * resolutionXMultiplier);
            
            //GeorgeCollisionBox.Width = Math.Ceiling(65 * resolutionXMultiplier);
            //GeorgeCollisionBox.Height = Math.Ceiling(43 * resolutionXMultiplier);

            georgeCollisionRect1.Width = Math.Ceiling(45 * resolutionXMultiplier);
            georgeCollisionRect1.Height = Math.Ceiling(42 * resolutionXMultiplier);
            
            georgeCollisionRect2.Width = Math.Ceiling(39 * resolutionXMultiplier);
            georgeCollisionRect2.Height = Math.Ceiling(44 * resolutionXMultiplier);
            
            georgeCollisionRect3.Width = Math.Ceiling(51 * resolutionXMultiplier);
            georgeCollisionRect3.Height = Math.Ceiling(42 * resolutionXMultiplier);

            double decalHeight = 55 * resolutionXMultiplier;
            damageDecalYCenter = decalHeight / 2;

            GeorgeDamageDecal.Width = 48 * resolutionXMultiplier;
            GeorgeDamageDecal.Height = 55 * resolutionXMultiplier;

            climbingUpPoint = height - (259 * resolutionXMultiplier);
            climbingDownPoint = -(285 * resolutionXMultiplier);

            // left punch offset
            georgeCollision1Offset = 102 * resolutionXMultiplier;
            georgeCollision2Offset = 117 * resolutionXMultiplier;
            georgeCollision3Offset = 244 * resolutionXMultiplier;
            georgeBodyCollisionOffset = 70 * resolutionXMultiplier;

            double georgeOffset = 0;

            if (portraitModeIndex == 1)
            {
                georgeOffset = Math.Ceiling(width - (1275 * resolutionXMultiplier));
            }
            else
            {
                georgeOffset = width - (1417 * resolutionXMultiplier);
            }
            
            Canvas.SetLeft(RampageMonsterRectangle, georgeOffset);
            
            georgeBodyCollisionRect.X = georgeOffset + (120 * resolutionXMultiplier);

            georgeCollisionRect1.X = georgeOffset;

            double george2Offset = (width - (totalBuildingsWidth * resolutionXMultiplier)) + (47 * resolutionXMultiplier);
            georgeCollisionRect2.X = george2Offset;

            georgeCollisionRect3.X = georgeOffset + (208 * resolutionXMultiplier);

            // set screen static dimensions
            //StaticImage.Viewport = new Rect(0, 0, Math.Ceiling(60 * resolutionXMultiplier), Math.Ceiling(78 * resolutionXMultiplier));
            StaticRectangle.Fill = staticBrushes[0];
            
            RtypePlasma.Width = 120 * resolutionXMultiplier;
            RtypePlasma.Height = 123 * resolutionXMultiplier;

            plasmaCollisionRect.Width = RtypePlasma.Width;
            plasmaCollisionRect.Height = RtypePlasma.Height;

            rtypePlasmaOffset = -(274 * resolutionXMultiplier);

            Canvas.SetLeft(RtypePlasma, rtypePlasmaOffset);

            // set rtype ship dimensions
            rtypeWidth = RTYPE_SHIP_AND_PLASMA_COMBINED_LENGTH * resolutionXMultiplier;
            rtypeShipOnlyLength = RTYPE_SHIP_LENGTH * resolutionXMultiplier;

            Rtype.Width = rtypeWidth;
            Rtype.Height = 119 * resolutionXMultiplier;

            // set rtype collision box dimensions
            //RtypeCollisionBox.Width = RtypePlasma.Width;
            //RtypeCollisionBox.Height = 50 * resolutionXMultiplier;

            rtypeCollisionRect.Width = 120 * resolutionXMultiplier;
            rtypeCollisionRect.Height = Rtype.Height;
                        
            // set starting rtype horizontal position
            Canvas.SetLeft(Rtype, -(rtypeWidth));

            // set starting rtype collision box position
            //Canvas.SetLeft(RtypeCollisionBox, -(rtypeWidth));

            // set rtype vertical positions
            rtypeVerticalPosition = (height / 2) - (Rtype.Height / 2);

            if (surroundPosition == BigBlue.Models.SurroundPosition.Up || surroundPosition == BigBlue.Models.SurroundPosition.Down)
            {
                double screen1Height = height - surroundGameListWidthOffset;

                //rtypeVerticalPosition = surroundGameListWidthOffset + ((screen1Height - bushHeight) /2) - (Rtype.Height / 2);

                rtypeVerticalPosition = surroundGameListWidthOffset + (screen1Height / 2) - (Rtype.Height / 2);

                // screen2 height + (screen1 height - bush) / 2

            }

            Canvas.SetTop(Rtype, rtypeVerticalPosition);
            Canvas.SetTop(RtypePlasma, rtypeVerticalPosition + (16 * resolutionXMultiplier));

            rtypeCollisionBoxVerticalPosition = rtypeVerticalPosition + (34 * resolutionXMultiplier);
            rtypeLaserVerticalPosition = rtypeCollisionBoxVerticalPosition + (26 * resolutionXMultiplier);

            rtypeCollisionRect.Y = rtypeCollisionBoxVerticalPosition;
            
            //Canvas.SetTop(RtypeCollisionBox, rtypeCollisionBoxVerticalPosition);

            rtypeLaserWidth = 60 * resolutionXMultiplier;
            rtypeLaserHeight = 17 * resolutionXMultiplier;

            currentLaserCollisionRect.Width = rtypeLaserWidth;
            currentLaserCollisionRect.Height = rtypeLaserHeight;

            // set foreground fighter dimensions
            FightRectangle.Width = Math.Ceiling(236 * resolutionXMultiplier);
            FightRectangle.Height = Math.Ceiling(86 * resolutionXMultiplier);

            DrawGameRectangle.Width = Math.Ceiling(221 * resolutionXMultiplier);
            DrawGameRectangle.Height = Math.Ceiling(126 * resolutionXMultiplier);

            DoubleKODecision.Width = Math.Ceiling(240 * resolutionXMultiplier);
            DoubleKODecision.Height = Math.Ceiling(145 * resolutionXMultiplier);

            WinnerRectangle.Width = Math.Ceiling(661 * resolutionXMultiplier);
            WinnerRectangle.Height = Math.Ceiling(48 * resolutionXMultiplier);

            FightersRectangle.Width = Math.Ceiling(1189 * resolutionXMultiplier);
            FightersRectangle.Height = Math.Ceiling(694 * resolutionXMultiplier);

            ForegroundObjects.Width = width;
            ForegroundObjects.Height = Math.Ceiling(694 * resolutionXMultiplier);

            Canvas.SetZIndex(FightersRectangle, 120);
            Canvas.SetRight(FightersRectangle, Math.Ceiling(157 * resolutionXMultiplier));
        }

        private void AnimateScreen()
        {
            DoubleAnimation screenAnimation = new DoubleAnimation();
            screenAnimation.From = 0;
            screenAnimation.To = 0.2;
            //screenAnimation.SpeedRatio = 8.0;
            screenAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(15));
            screenAnimation.RepeatBehavior = RepeatBehavior.Forever;
            screenAnimation.AutoReverse = true;

            Storyboard.SetTarget(screenAnimation, ScreenDoor);
            Storyboard.SetTargetProperty(screenAnimation, new PropertyPath("(Opacity)"));

            screenAnimation.Freeze();

            ScreenStoryBoard.Children.Add(screenAnimation);
            ScreenStoryBoard.Freeze();
            ScreenStoryBoard.Begin();
        }
                
        List<Rectangle> bushTiles;
        List<ImageBrush> bushImageBrushes = new List<ImageBrush>(3);
        
        private void ProvisionBush()
        {
            for (int i = 0; i < 4; i++)
            {
                ImageBrush bushImageBrush = new ImageBrush();
                bushImageBrush.TileMode = TileMode.None;

                if (integerMultiplier == true)
                {
                    RenderOptions.SetBitmapScalingMode(bushImageBrush, BitmapScalingMode.NearestNeighbor);
                    RenderOptions.SetEdgeMode(bushImageBrush, EdgeMode.Aliased);
                }

                bushImageBrush.ImageSource = BigBlue.ImageLoading.loadImage("bushes" + i.ToString() + ".png", integerMultiplier);

                bushImageBrush.Freeze();
                bushImageBrushes.Add(bushImageBrush);
            }

            double bushWidth = 1440 * resolutionXMultiplier;
            double numberOfBushTiles = Math.Ceiling(width / bushWidth);

            bushTiles = new List<Rectangle>(Convert.ToInt32(numberOfBushTiles - 1));

            for (int i = 0; i < numberOfBushTiles; i++)
            {
                Rectangle bushRectangle = new Rectangle();
                bushRectangle.IsHitTestVisible = false;
                Panel.SetZIndex(bushRectangle, 99);
                bushRectangle.Width = bushWidth;
                bushRectangle.Height = bushHeight;

                bushRectangle.Fill = bushImageBrushes[timeCode];

                ForegroundObjects.Children.Add(bushRectangle);

                if (integerMultiplier == true)
                {
                    RenderOptions.SetBitmapScalingMode(bushRectangle, BitmapScalingMode.NearestNeighbor);
                    RenderOptions.SetEdgeMode(bushRectangle, EdgeMode.Aliased);

                }
                bushTiles.Add(bushRectangle);

                Canvas.SetBottom(bushRectangle, 0);

                Canvas.SetRight(bushRectangle, bushWidth * i);
            }
        }

        List<Rectangle> spectatorTiles;

        List<List<List<ImageBrush>>> spectatorAnimations = new List<List<List<ImageBrush>>>(3);

        private void ProvisionSpectators()
        {
            List<List<Rect>> spectatorViewboxes = new List<List<Rect>>(2);

            List<Rect> championEditionFrames = new List<Rect>(4);
            championEditionFrames.Add(new Rect(0, 0, 480, 381));
            championEditionFrames.Add(new Rect(0, 385, 480, 381));
            championEditionFrames.Add(new Rect(0, 0, 480, 381));
            championEditionFrames.Add(new Rect(0, 770, 480, 381));

            spectatorViewboxes.Add(championEditionFrames);

            List<Rect> hyperFightingFrames = new List<Rect>(4);
            hyperFightingFrames.Add(new Rect(0, 0, 480, 381));
            hyperFightingFrames.Add(new Rect(0, 385, 480, 381));
            hyperFightingFrames.Add(new Rect(0, 770, 480, 381));
            hyperFightingFrames.Add(new Rect(0, 1155, 480, 381));

            spectatorViewboxes.Add(hyperFightingFrames);

            for (int sfEditionIndex = 0; sfEditionIndex < 2; sfEditionIndex++)
            {
                List<List<ImageBrush>> spectatorTimesOfDay = new List<List<ImageBrush>>(3);
                for (int timeIndex = 0; timeIndex < 4; timeIndex++)
                {
                    List<ImageBrush> spectatorFrames = new List<ImageBrush>(4);

                    for (int frameIndex = 0; frameIndex < 4; frameIndex++)
                    {
                        ImageBrush spectatorBrush = new ImageBrush();
                        spectatorBrush.ImageSource = BigBlue.ImageLoading.loadImage("spectators" + sfEditionIndex.ToString() + timeIndex.ToString() + ".png", integerMultiplier);
                        spectatorBrush.Viewbox = spectatorViewboxes[sfEditionIndex][frameIndex];
                        spectatorBrush.ViewboxUnits = BrushMappingMode.Absolute;
                        spectatorBrush.TileMode = TileMode.None;

                        if (integerMultiplier == true)
                        {
                            RenderOptions.SetBitmapScalingMode(spectatorBrush, BitmapScalingMode.NearestNeighbor);
                            RenderOptions.SetEdgeMode(spectatorBrush, EdgeMode.Aliased);
                        }

                        spectatorBrush.Freeze();

                        spectatorFrames.Add(spectatorBrush);
                    }

                    spectatorTimesOfDay.Add(spectatorFrames);
                }
                spectatorAnimations.Add(spectatorTimesOfDay);
            }

            double spectatorsWidth = Math.Ceiling(480 * resolutionXMultiplier);
            double spectatorsHeight = Math.Ceiling(381 * resolutionXMultiplier);
            double numberOfSpectatorTiles = Math.Ceiling(width / spectatorsWidth);

            spectatorTiles = new List<Rectangle>(Convert.ToInt32(numberOfSpectatorTiles - 1));

            for (int i = 0; i < numberOfSpectatorTiles; i++)
            {
                Rectangle spectatorRectangle = new Rectangle();
                spectatorRectangle.IsHitTestVisible = false;
                Panel.SetZIndex(spectatorRectangle, 100);
                spectatorRectangle.Width = spectatorsWidth;
                spectatorRectangle.Height = spectatorsHeight;
                spectatorRectangle.Fill = spectatorAnimations[streetFighterEditionAnimation][timeCode][0];

                ForegroundObjects.Children.Add(spectatorRectangle);

                if (integerMultiplier == true)
                {
                    RenderOptions.SetBitmapScalingMode(ForegroundObjects, BitmapScalingMode.NearestNeighbor);
                    RenderOptions.SetEdgeMode(ForegroundObjects, EdgeMode.Aliased);

                    RenderOptions.SetBitmapScalingMode(spectatorRectangle, BitmapScalingMode.NearestNeighbor);
                    RenderOptions.SetEdgeMode(spectatorRectangle, EdgeMode.Aliased);
                }

                spectatorTiles.Add(spectatorRectangle);

                Canvas.SetBottom(spectatorRectangle, 0);
                Canvas.SetRight(spectatorRectangle, spectatorsWidth * i);
            }
        }

        private void ProvisionVersusScreens()
        {
            ProvisionVersusModeAnimations();

            double versusPortraitWidth = 1362 * resolutionXMultiplier;
            double versusPortraitHeight = 749 * resolutionXMultiplier;

            VersusPortraits.Width = versusPortraitWidth;
            VersusPortraits.Height = versusPortraitHeight;
            

            if (surroundPosition != BigBlue.Models.SurroundPosition.None)
            {
                VersusPortraitsSecondary.Visibility = Visibility.Visible;
                VersusPortraitsSecondary.Width = versusPortraitWidth;
                VersusPortraitsSecondary.Height = versusPortraitHeight;
            }
            
            switch (surroundPosition)
            {
                case BigBlue.Models.SurroundPosition.Up:
                case BigBlue.Models.SurroundPosition.Down:
                    Canvas.SetLeft(VersusPortraits, (width / 2) - (versusPortraitWidth / 2));
                    Canvas.SetLeft(VersusPortraitsSecondary, (width / 2) - (versusPortraitWidth / 2));
                    Canvas.SetTop(VersusPortraits, (surroundGameListWidthOffset / 2) - (versusPortraitHeight / 2));
                    Canvas.SetBottom(VersusPortraitsSecondary, ((height - surroundGameListWidthOffset) / 2) - (versusPortraitHeight / 2));
                    
                    break;
                case BigBlue.Models.SurroundPosition.Left:
                case BigBlue.Models.SurroundPosition.Right:
                    Canvas.SetLeft(VersusPortraits, (surroundGameListWidthOffset / 2) - (versusPortraitWidth / 2));
                    Canvas.SetLeft(VersusPortraitsSecondary, (surroundGameListWidthOffset + ((width - surroundGameListWidthOffset) / 2)) - (versusPortraitWidth / 2));
                    Canvas.SetTop(VersusPortraits, (height / 2) - (versusPortraitHeight / 2));
                    Canvas.SetTop(VersusPortraitsSecondary, (height / 2) - (versusPortraitHeight / 2));
                    break;
                default:
                    Canvas.SetLeft(VersusPortraits, (width / 2) - (versusPortraitWidth / 2));
                    Canvas.SetTop(VersusPortraits, (height / 2) - (versusPortraitHeight / 2));
                    break;
            }
            
            //Rect VersusViewboxRect = new Rect(0, 0, 1362, 749);
            BrushMappingMode VersusViewboxUnits = BrushMappingMode.Absolute;
            //Rect VersusViewPortRect = new Rect((width - (Math.Ceiling(1362 * resolutionXMultiplier))) / 2, (height - (Math.Ceiling(749 * resolutionXMultiplier))) / 2, Math.Ceiling(1362 * resolutionXMultiplier), Math.Ceiling(749 * resolutionXMultiplier));
            BrushMappingMode VersusViewPortUnits = BrushMappingMode.Absolute;

            string versusImageModifier = string.Empty;

            if (streetFighterEdition == 0)
            {
                versusImageModifier = "ww";
            }
                
            versusBgBrush.Add(BigBlue.ImageLoading.loadImageBrush("versus0" + versusImageModifier + ".png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, VersusViewboxUnits, nullRect, VersusViewPortUnits, true, integerMultiplier));
            versusBgBrush.Add(BigBlue.ImageLoading.loadImageBrush("versus1" + versusImageModifier + ".png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, VersusViewboxUnits, nullRect, VersusViewPortUnits, true, integerMultiplier));
            versusBgBrush.Add(BigBlue.ImageLoading.loadImageBrush("versus2" + versusImageModifier + ".png", TileMode.None, AlignmentX.Left, AlignmentY.Top, nullRect, VersusViewboxUnits, nullRect, VersusViewPortUnits, true, integerMultiplier));
        }

        ImageBrush[] bgBuildings = new ImageBrush[4];

        private void ProvisionBgBuilding()
        {
            for (int i = 0; i < 4; i++)
            {
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = BigBlue.ImageLoading.loadImage("bgbuildings" + i.ToString() + ".png", integerMultiplier);
                ib.AlignmentX = AlignmentX.Right;
                ib.AlignmentY = AlignmentY.Bottom;
                ib.TileMode = TileMode.None;
                // set render options maybe?
                RenderOptions.SetBitmapScalingMode(ib, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetCachingHint(ib, CachingHint.Cache);
                ib.Freeze();
                bgBuildings[i] = ib;
            }
        }

        List<List<ImageBrush>> buildingBlockAnimationFrames = new List<List<ImageBrush>>(4);
        List<List<ImageBrush>> buildingDebrisAnimationFrames = new List<List<ImageBrush>>(4);

        private void ProvisionBuildingHolesAndDebris()
        {
            BitmapImage destructionImageDay = BigBlue.ImageLoading.loadImage("destruction.png", integerMultiplier);
            BitmapImage destructionImageSunset = BigBlue.ImageLoading.loadImage("destruction_sunset.png", integerMultiplier);
            BitmapImage destructionImageNight = BigBlue.ImageLoading.loadImage("destruction_night.png", integerMultiplier);
            BitmapImage destructionImageDawn = BigBlue.ImageLoading.loadImage("destruction_dawn.png", integerMultiplier);
            
            for (int i = 0; i < 4; i++)
            {
                List<ImageBrush> debriBrushes = new List<ImageBrush>(4);
                buildingDebrisAnimationFrames.Add(debriBrushes);

                List<ImageBrush> blockBrushes = new List<ImageBrush>(16);
                buildingBlockAnimationFrames.Add(blockBrushes);
            }

            Rect[] damageRects = new Rect[16];
            damageRects[0] = new Rect(-300, -300, 0, 0);
            damageRects[1] = new Rect(0, 0, 124, 91);
            damageRects[2] = new Rect(0, 190, 124, 91);
            damageRects[3] = new Rect(0, 285, 124, 91);
            damageRects[4] = new Rect(0, 95, 124, 91);
            damageRects[5] = new Rect(0, 380, 124, 91);
            damageRects[6] = new Rect(128, 0, 124, 91);
            damageRects[7] = new Rect(128, 95, 124, 91);
            damageRects[8] = new Rect(128, 190, 124, 91);
            damageRects[9] = new Rect(128, 285, 124, 91);
            damageRects[10] = new Rect(128, 380, 124, 91);
            damageRects[11] = new Rect(128, 0, 124, 91);
            damageRects[12] = new Rect(256, 95, 124, 91);
            damageRects[13] = new Rect(256, 190, 124, 91);
            damageRects[14] = new Rect(256, 285, 124, 91);
            damageRects[15] = new Rect(256, 380, 124, 91);

            foreach (Rect d in damageRects)
            {
                buildingBlockAnimationFrames[0].Add(BigBlue.ImageLoading.loadAnimationFrame(destructionImageDay, d, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                buildingBlockAnimationFrames[1].Add(BigBlue.ImageLoading.loadAnimationFrame(destructionImageSunset, d, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                buildingBlockAnimationFrames[2].Add(BigBlue.ImageLoading.loadAnimationFrame(destructionImageNight, d, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                buildingBlockAnimationFrames[3].Add(BigBlue.ImageLoading.loadAnimationFrame(destructionImageDawn, d, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
            }

            Rect[] debrisRects = new Rect[4];
            debrisRects[0] = new Rect(0, 475, 220, 72);
            debrisRects[1] = new Rect(0, 551, 220, 72);
            debrisRects[2] = new Rect(0, 627, 220, 72);
            debrisRects[3] = new Rect(0, 703, 220, 72);

            foreach (Rect d in debrisRects)
            {
                buildingDebrisAnimationFrames[0].Add(BigBlue.ImageLoading.loadAnimationFrame(destructionImageDay, d, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                buildingDebrisAnimationFrames[1].Add(BigBlue.ImageLoading.loadAnimationFrame(destructionImageSunset, d, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                buildingDebrisAnimationFrames[2].Add(BigBlue.ImageLoading.loadAnimationFrame(destructionImageNight, d, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
                buildingDebrisAnimationFrames[3].Add(BigBlue.ImageLoading.loadAnimationFrame(destructionImageDawn, d, BrushMappingMode.Absolute, TileMode.None, AlignmentX.Left, AlignmentY.Top, integerMultiplier));
            }
        }

        private const double BASE_FRONTEND_WIDTH = 1920;
        private const double BASE_FRONTEND_HEIGHT = 1080;

        private double horizontalAnimationSpeedRatio = 1;
        private double verticalAnimationSpeedRatio = 1;

        private const double RTYPE_WARP_FACTOR = 4;

        private void CalculateAnimationSpeedRatios()
        {
            double actualBuildingToRampageMonsterRatio = FrontEndContainer.Height / (RAMPAGE_MONSTER_HEIGHT * resolutionXMultiplier);

            verticalAnimationSpeedRatio = EXPECTED_HEIGHT_TO_RAMPAGE_MONSTER_RATIO / actualBuildingToRampageMonsterRatio;

            double actualWidthToRtypeRatio = FrontEndContainer.Width / (RTYPE_SHIP_LENGTH * resolutionXMultiplier);

            double horizontalAnimationSpeedRatio = EXPECTED_WIDTH_TO_RTYPE_RATIO / actualWidthToRtypeRatio;
        }

        private void ProvisionRampageClimbingAnimation()
        {
            DoubleAnimation georgeClimbingAnimation = new DoubleAnimation();
            georgeClimbingAnimation.From = height - (250 * resolutionXMultiplier);
            georgeClimbingAnimation.To = -(310 * resolutionXMultiplier);
            georgeClimbingAnimation.SpeedRatio = verticalAnimationSpeedRatio;
            georgeClimbingAnimation.Duration = new Duration(TimeSpan.FromSeconds(10));
            georgeClimbingAnimation.AutoReverse = true;
            georgeClimbingAnimation.RepeatBehavior = RepeatBehavior.Forever;
            
            Storyboard.SetTarget(georgeClimbingAnimation, RampageMonsterRectangle);

            // Set the attached properties of Canvas.Left and Canvas.Top
            // to be the target properties of the two respective DoubleAnimations.
            Storyboard.SetTargetProperty(georgeClimbingAnimation, new PropertyPath("(Canvas.Top)"));

            georgeClimbingAnimation.Freeze();

            GeorgeClimbingStoryBoard.Children.Add(georgeClimbingAnimation);
            GeorgeClimbingStoryBoard.Freeze();
        }



        private void ProvisionRtypeFlyingAnimation()
        {
            DoubleAnimation rtypeFlyingAnimation = new DoubleAnimation();
            rtypeFlyingAnimation.From = -(rtypeWidth);
            rtypeFlyingAnimation.To = width + (300 * resolutionXMultiplier);
            rtypeFlyingAnimation.SpeedRatio = horizontalAnimationSpeedRatio;
            rtypeFlyingAnimation.Duration = new Duration(TimeSpan.FromSeconds(15));
            rtypeFlyingAnimation.AutoReverse = false;
            rtypeFlyingAnimation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTarget(rtypeFlyingAnimation, Rtype);

            Storyboard.SetTargetProperty(rtypeFlyingAnimation, new PropertyPath("(Canvas.Left)"));
            rtypeFlyingAnimation.Freeze();

            RtypeFlyingStoryboard.Children.Add(rtypeFlyingAnimation);
        }

        

        

        private void ProvisionVersusModeAnimations()
        {
            DoubleAnimation versusFadeInAnimation = new DoubleAnimation();
            versusFadeInAnimation.From = 0;
            versusFadeInAnimation.To = 1;
            versusFadeInAnimation.SpeedRatio = 1;
            versusFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            versusFadeInAnimation.AutoReverse = false;

            Storyboard.SetTarget(versusFadeInAnimation, VsOverlay);
            Storyboard.SetTargetProperty(versusFadeInAnimation, new PropertyPath("Opacity"));
            versusFadeInAnimation.Freeze();

            ShowVersusStoryboard.Children.Add(versusFadeInAnimation);
            ShowVersusStoryboard.Completed += VersusShown;
            ShowVersusStoryboard.Freeze();

            DoubleAnimation versusFadeOutAnimation = new DoubleAnimation();
            versusFadeOutAnimation.From = 1;
            versusFadeOutAnimation.To = 0;
            versusFadeOutAnimation.SpeedRatio = 1;
            versusFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            versusFadeOutAnimation.AutoReverse = false;

            Storyboard.SetTarget(versusFadeOutAnimation, VsOverlay);
            Storyboard.SetTargetProperty(versusFadeOutAnimation, new PropertyPath("Opacity"));
            versusFadeOutAnimation.Freeze();

            HideVersusStoryboard.Children.Add(versusFadeOutAnimation);
            HideVersusStoryboard.Completed += VersusHidden;
            HideVersusStoryboard.Freeze();
        }

        

        private void SetImageScalingMode()
        {
            if (width >= baseHorizontalResolution && IsInteger(resolutionXMultiplier))
            {
                integerMultiplier = true;

                RenderOptions.SetBitmapScalingMode(SnapShotRectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(Video, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(VideoElement, BitmapScalingMode.NearestNeighbor);

                RenderOptions.SetBitmapScalingMode(ClockCanvas, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(HourOneRectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(HourTwoRectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(ColonRectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(MinuteOneRectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(MinuteTwoRectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(TwelveHourOneRectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(TwelveHourTwoRectangle, BitmapScalingMode.NearestNeighbor);

                //RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
                RenderOptions.SetBitmapScalingMode(Cloud1Rectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(Cloud2Rectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(Cloud3Rectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(Cloud4Rectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(Cloud5Rectangle, BitmapScalingMode.NearestNeighbor);

                RenderOptions.SetEdgeMode(FrontEndWindow, EdgeMode.Aliased);
                RenderOptions.SetEdgeMode(LayoutRoot, EdgeMode.Aliased);
                //RenderOptions.SetEdgeMode(MainMenuContainer, EdgeMode.Aliased);
                RenderOptions.SetEdgeMode(FrontEndContainer, EdgeMode.Aliased);

                RenderOptions.SetBitmapScalingMode(KOCountdown, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(KOCountdown, EdgeMode.Aliased);

                RenderOptions.SetBitmapScalingMode(FightersRectangle, BitmapScalingMode.NearestNeighbor);

                RenderOptions.SetBitmapScalingMode(ScreenDoor, BitmapScalingMode.NearestNeighbor);

                RenderOptions.SetBitmapScalingMode(StaticRectangle, BitmapScalingMode.NearestNeighbor);
                
                RenderOptions.SetEdgeMode(HaggarCanvas, EdgeMode.Aliased);
                RenderOptions.SetEdgeMode(HaggarBackgroundRectangle, EdgeMode.Aliased);
                RenderOptions.SetEdgeMode(HaggarHeadRectangle, EdgeMode.Aliased);
                RenderOptions.SetEdgeMode(HaggarForegroundRectangle, EdgeMode.Aliased);

                RenderOptions.SetBitmapScalingMode(HaggarCanvas, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(HaggarBackgroundRectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(HaggarHeadRectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(HaggarForegroundRectangle, BitmapScalingMode.NearestNeighbor);

                //RenderOptions.SetBitmapScalingMode(Snapshot, BitmapScalingMode.NearestNeighbor);
                //RenderOptions.SetBitmapScalingMode(HaggarImage, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(KnifeRectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(KnifeImage, BitmapScalingMode.NearestNeighbor);

                RenderOptions.SetBitmapScalingMode(BgBuildingsRectangle, BitmapScalingMode.NearestNeighbor);

                RenderOptions.SetBitmapScalingMode(ScreenBackgroundRectangle, BitmapScalingMode.NearestNeighbor);

                RenderOptions.SetBitmapScalingMode(BuildingRoofRectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(BuildingRectangle, BitmapScalingMode.NearestNeighbor);

                RenderOptions.SetBitmapScalingMode(Rtype, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(Rtype, EdgeMode.Aliased);
                RenderOptions.SetBitmapScalingMode(RtypePlasma, BitmapScalingMode.NearestNeighbor);

                RenderOptions.SetBitmapScalingMode(WinnerRectangle, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(WinnerRectangle, EdgeMode.Aliased);
                RenderOptions.SetBitmapScalingMode(WinnerImage, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(WinnerImage, EdgeMode.Aliased);

                RenderOptions.SetBitmapScalingMode(RampageMonsterRectangle, BitmapScalingMode.NearestNeighbor);
            }
        }
        
        protected override void FinalListRender(MediaElement videoMe, System.Windows.Controls.Panel videoCanvas)
        {
            UpdateTextBlockText();

            /*   
            await Task.Run(() => { SetGameSnapshots(false); });

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                stopVideo();
                resetVideo();
            });
            */

            SetGameSnapshots(false);
            
            StopVideo();

            ResetVideo();
        }

        internal override void InitializeSound()
        {
            BigBlue.XAudio2Player.ProvisionPlayer(miniGameVolume);

            //m_MasteringVoice.GetChannelMask(out speakers);
            
            if (!BigBlue.XAudio2Player.Disabled)
            {
                BigBlue.XAudio2Player.AddAudioFile("all", ExitListSoundKey, "back.wav", false, false, true, null);
                BigBlue.XAudio2Player.AddAudioFile("all", "crash", "crash.wav", false, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile("center", "breakwindow", "breakwindow.wav", false, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile("right", "buildingcrack1", "buildingcrack1.wav", false, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile("right", "buildingcrack2", "buildingcrack2.wav", false, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile("all", "buildingexplosion", "buildingexplosion.wav", false, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile("all", "buildingstoreycrumble", "buildingstoreycrumble.wav", false, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile("all", ListNavigationSoundKey, "cursormove.wav", false, false, true, null);

                BigBlue.XAudio2Player.AddAudioFile("right", "fuse", "fuse.wav", true, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile("center", "georgescream", "georgescream.wav", false, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile("right", "knife", "knife.wav", false, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile(null, "laser", "laser.wav", false, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile("center", "lightpunch", "lightpunch.wav", false, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile("center", "punch", "punch.wav", false, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile("all", SelectListSoundKey, "selectcategory.wav", false, false, true, null);

                BigBlue.XAudio2Player.AddAudioFile("center", "swing", "swing.wav", false, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile(null, "warp", "warp.wav", true, true, true, null);
                BigBlue.XAudio2Player.AddAudioFile("center", "wtf", "wtf.wav", false, true, true, null);

                BigBlue.XAudio2Player.AddAudioFile(null, "explosion", "explosion.wav", false, true, true, RtypeExplosionComplete);
                BigBlue.XAudio2Player.AddAudioFile("all", LaunchListItemSoundKey, "selectgame.wav", false, true, true, BeginGameLaunch);
                BigBlue.XAudio2Player.AddAudioFile("center", FailureSoundKey, "youlose.wav", false, true, true, HumiliationFinished);

                // we only need these sounds if the free for all mode is off
                if (freeForAll == false)
                {
                    BigBlue.XAudio2Player.AddAudioFile("center", "fight", "fight.wav", false, true, true, FightAnnouncementCleanup);
                    BigBlue.XAudio2Player.AddAudioFile("all", "georgevictory", "georgevictory.wav", false, true, true, VictoryCleanup);
                    BigBlue.XAudio2Player.AddAudioFile("all", "newchallenger", "newchallenger.wav", false, true, true, NewChallengerCleanup);
                    BigBlue.XAudio2Player.AddAudioFile("all", "rtypevictory", "rtypevictory.wav", false, true, true, VictoryCleanup);
                    BigBlue.XAudio2Player.AddAudioFile("all", "versus", "versus.wav", false, true, true, HideVersusScreen);

                    // only need the guile song when free for all mode is off and music is on
                    if (music == true)
                    {
                        BigBlue.XAudio2Player.AddAudioFile("all", "guile", "guile.wav", true, true, true, null);
                        BigBlue.XAudio2Player.AddAudioFile("all", "guilefast", "guilefast.wav", true, true, true, null);
                    }
                }

                // only need the guile song when free for all mode is off and music is on
                if (music == true)
                {
                    BigBlue.XAudio2Player.AddAudioFile("all", MusicSoundKey, "characterselect.wav", false, true, true, ResetVideoElementVolume);
                }
            }
        }

        internal override void InitializeFrontEnd()
        {
            InitializeSound();
            
            // check for time changes every 1 minute
            timeCodeTimer.Interval = new TimeSpan(0, 0, 1, 0, 0);
            timeCodeTimer.Tick += TimeCodeTimer_Tick;
            timeCodeTimer.Start();

            ProvisionMainWindow(LayoutRoot, FrontEndContainer);

            // freeze all the brushes you can
            transparentBrush.Freeze();
            whiteBrush.Freeze();
            yellowBrush.Freeze();
            blueBrush.Freeze();
            blackBrush.Freeze();
            transparentBlueBrush.Freeze();
            
            morningBrush.Freeze();
            sunsetBrush.Freeze();
            nightBrush.Freeze();

            if (portraitModeIndex == 1)
            {
                totalBuildingsWidth = 1058;
                chunksPerTick = 2;
            }
            
            SetImageScalingMode();

            SetResolutionSpecificDimensions();

            try
            {
                if (aspectRatioIndex == 2)
                {
                    versusThumbnailImage = BigBlue.ImageLoading.loadImage("versusthumbnail" + streetFighterEdition + "2.png", integerMultiplier);
                }
                else
                {
                    versusThumbnailImage = BigBlue.ImageLoading.loadImage("versusthumbnail" + streetFighterEdition + portraitModeIndex + ".png", integerMultiplier);
                }

                speechThumbnailImage = BigBlue.ImageLoading.loadImage("speech" + portraitModeIndex + ".png", integerMultiplier);
            }
            catch (Exception)
            {
            }

            if (stretchSnapshots)
            {
                SnapShotRectangle.Stretch = Stretch.Fill;
                VideoElement.Stretch = Stretch.Fill;

                if (marqueeDisplay)
                {
                    marqueeWindow.SecondaryWindowSnapshot.Stretch = Stretch.Fill;
                }

                if (instructionDisplay)
                {
                    instructionWindow.SecondaryWindowSnapshot.Stretch = Stretch.Fill;
                }

                if (flyerDisplay)
                {
                    flyerWindow.SecondaryWindowSnapshot.Stretch = Stretch.Fill;
                }
            }

            SnapShotBackgroundRectangle.Fill = blackBrush;

            // we use the same versus background image for both champion edition and hyperfighting
            if (streetFighterEdition == 0)
            {
                VsOverlay.Background = blackBrush;
                MainMenu.Background = blackBrush;
            }
            else
            {
                MainMenu.Background = blueBrush;
            }
            
            LoadFrontendList();
            
            AddGameListTextBlocks(GameList, GameListOutlines, blackBrush);
                        
            ProvisionUIAnimations(LayoutRoot, FrontEndContainer);

            ProvisionBgBuilding();

            // burning image
            rampageBurning = BigBlue.ImageLoading.loadImage("rampageburning.png", integerMultiplier);

            lifeBarNameRects.Add(new Rect(4, 4, 211, 44));
            lifeBarNameRects.Add(new Rect(4, 52, 211, 44));
            lifeBarNameRects.Add(new Rect(4, 100, 211, 44));
            lifeBarNameRects.Add(new Rect(4, 148, 211, 44));

            TwoNameImage.Viewbox = lifeBarNameRects[3];

            CalculateAnimationSpeedRatios();

            ProvisionFighterBlood();
            ProvisionFighters();
            ProvisionRampageMonsters();

            ProvisionLaser();
            ProvisionPlasma();
            ProvisionRtype();
            ProvisionNumbers();
            ProvisionHaggar();

            ProvisionSky();
            ProvisionMainBuilding();
            ProvisionBush();
            ProvisionSpectators();

            RenderTimeBasedOnCalculation(false);

            AnimateSky();

            RenderGameList(false);
            //calculateTime(true);

            // you have to provision the screens before you can choose an initial character
            ProvisionVersusScreens();

            // choose the initial rampage character
            ChooseRampageCharacter(false);

            newChallengerRects.Add(new Rect(0, 0, 586, 135));
            newChallengerRects.Add(new Rect(0, 139, 586, 135));

            ProvisionBuildings();
            ProvisionKnockOut();
            ProvisionWinnerText();
            
            // rtype isn't going to be alive at the beginning of the program anymore
            //resetRtype();
            //rtypeTimer.Start();
            //staticTimer.Start();
            //haggarTimer.Start();
            //igniteTimer.Start();
            //fuseTimer.Start();

            if (snapshotFlicker == true)
            {
                AnimateScreen();
            }
            else
            {
                ScreenDoor.Visibility = System.Windows.Visibility.Hidden;
            }
            
            ProvisionBuildingHolesAndDebris();

            AddBuildingObjects();

            // get the regions of the screen for speaker assignments
            oneThirdScreenWidth = width / 3;
            twoThirdsScreenWidth = oneThirdScreenWidth * 2;
            
            // start timers
            if (screenSaverTimeInMinutes >= 1)
            {
                screenSaverTimer.Start();
            }

            genericAnimationStopWatch.Start();

            //georgeTimer.Start();
            //fightersTimer.Start();
            //crowdTimer.Start();
            //plasmaTimer.Start();
            //buildingTimer.Start();

            /*
            foreach (Stopwatch debrisTimer in debrisTimers)
            {
                debrisTimer.Start();
            }
            */
            
            ProvisionShutDownSequence();
            
            CompositionTarget.Rendering += OnFrame;

            // prevent the windows cursor from getting out of the window
            this.Activated += MainWindow_Activated;

            SpawnRampageMonster(true);

            if (freeForAll == true)
            {
                RespawnRtype(true);
            }
            //else
            //{
            //    uiTimer.Start();
           // }

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            ShowFrontEndStoryboard.Begin();

            BigBlue.NativeMethods.SetForegroundWindow(windowHandle);
        }

        private void ProvisionShutDownSequence()
        {
            try
            {
                ObjectAnimationUsingKeyFrames showInitialStaticAnimation = new ObjectAnimationUsingKeyFrames();

                showInitialStaticAnimation.Duration = new TimeSpan(0, 0, 0, 1, 1);
                //animate.RepeatBehavior = RepeatBehavior.Forever;

                DiscreteObjectKeyFrame showStaticScreenKeyFrame = new DiscreteObjectKeyFrame(
                    Visibility.Visible,
                    new TimeSpan(0, 0, 0, 0, 1));
                showStaticScreenKeyFrame.Freeze();
                showInitialStaticAnimation.KeyFrames.Add(showStaticScreenKeyFrame);

                DiscreteObjectKeyFrame hideStaticScreenKeyFrame = new DiscreteObjectKeyFrame(
                    Visibility.Hidden,
                    new TimeSpan(0, 0, 0, 1, 1));
                hideStaticScreenKeyFrame.Freeze();
                showInitialStaticAnimation.KeyFrames.Add(hideStaticScreenKeyFrame);

                shutdownStoryboard.Completed += ShutdownStoryboard_Completed;

                Storyboard.SetTarget(showInitialStaticAnimation, StaticRectangle);
                Storyboard.SetTargetProperty(showInitialStaticAnimation, new PropertyPath("(Rectangle.Visibility)"));

                showInitialStaticAnimation.Freeze();

                shutdownStoryboard.Children.Add(showInitialStaticAnimation);
                shutdownStoryboard.Freeze();
                
                ObjectAnimationUsingKeyFrames regularlyScheduledProgrammingAnimation = new ObjectAnimationUsingKeyFrames();

                regularlyScheduledProgrammingAnimation.Duration = new TimeSpan(0, 0, 0, 2, 500);
                //animate.RepeatBehavior = RepeatBehavior.Forever;

                DiscreteObjectKeyFrame showStaticScreenKeyFrame2 = new DiscreteObjectKeyFrame(
                    Visibility.Visible,
                    new TimeSpan(0, 0, 0, 1, 500));
                showStaticScreenKeyFrame2.Freeze();
                regularlyScheduledProgrammingAnimation.KeyFrames.Add(showStaticScreenKeyFrame2);

                DiscreteObjectKeyFrame hideStaticScreenKeyFrame2 = new DiscreteObjectKeyFrame(
                    Visibility.Hidden,
                    new TimeSpan(0, 0, 0, 2, 500));
                hideStaticScreenKeyFrame2.Freeze();
                regularlyScheduledProgrammingAnimation.KeyFrames.Add(hideStaticScreenKeyFrame2);

                regularlyScheduledProgrammingStoryboard.Completed += RegularlyScheduledProgrammingStoryboard_Completed;

                Storyboard.SetTarget(regularlyScheduledProgrammingAnimation, StaticRectangle);
                Storyboard.SetTargetProperty(regularlyScheduledProgrammingAnimation, new PropertyPath("(Rectangle.Visibility)"));

                regularlyScheduledProgrammingAnimation.Freeze();
                regularlyScheduledProgrammingStoryboard.Children.Add(regularlyScheduledProgrammingAnimation);
                regularlyScheduledProgrammingStoryboard.Freeze();
                
                ObjectAnimationUsingKeyFrames staticDuringExplosionAnimation = new ObjectAnimationUsingKeyFrames();

                staticDuringExplosionAnimation.Duration = new TimeSpan(0, 0, 0, 0, 300);
                //staticDuringExplosionAnimation.RepeatBehavior = RepeatBehavior.Forever;

                DiscreteObjectKeyFrame showStaticDuringExplosionAnimation = new DiscreteObjectKeyFrame(
                    Visibility.Visible,
                    new TimeSpan(0, 0, 0, 0, 300));
                staticDuringExplosionAnimation.KeyFrames.Add(showStaticDuringExplosionAnimation);
                
                staticAfterExplosionStoryboard.Completed += StaticAfterExplosionStoryboard_Completed;

                Storyboard.SetTarget(staticDuringExplosionAnimation, StaticRectangle);
                Storyboard.SetTargetProperty(staticDuringExplosionAnimation, new PropertyPath("(Rectangle.Visibility)"));

                staticDuringExplosionAnimation.Freeze();

                staticAfterExplosionStoryboard.Children.Add(staticDuringExplosionAnimation);


                staticAfterExplosionStoryboard.Freeze();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error provisioning shutdown sequence: " + ex.Message, "Big Blue");
            }
        }

        

        private void ProvisionSunsetAnimation(SolidColorBrush brush1ToAnimate, SolidColorBrush brush2ToAnimate, SolidColorBrush brush3ToAnimate, SolidColorBrush brush4ToAnimate)
        {
            // sun animation for sunset
            DoubleAnimation cloud3Animation = new DoubleAnimation();
            cloud3Animation.From = width;
            cloud3Animation.To = -(2021 * resolutionXMultiplier);
            cloud3Animation.SpeedRatio = 0.15;
            cloud3Animation.Duration = new Duration(TimeSpan.FromSeconds(10));
            cloud3Animation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTarget(cloud3Animation, Cloud3Rectangle);
            Storyboard.SetTargetProperty(cloud3Animation, new PropertyPath("(Canvas.Left)"));

            cloud3Animation.Freeze();

            Cloud3StoryBoard.Children.Add(cloud3Animation);
            Cloud3StoryBoard.Freeze();

            ColorAnimation stippledAnimation1 = new ColorAnimation();
            stippledAnimation1.From = morningSkyColor2;
            stippledAnimation1.To = sunsetSkyColor2;
            stippledAnimation1.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation stippledAnimation2 = new ColorAnimation();
            stippledAnimation2.From = morningSkyColor3;
            stippledAnimation2.To = sunsetSkyColor3;
            stippledAnimation2.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation stippledAnimation3 = new ColorAnimation();
            stippledAnimation3.From = morningSkyColor4;
            stippledAnimation3.To = sunsetSkyColor4;
            stippledAnimation3.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation stippledAnimation4 = new ColorAnimation();
            stippledAnimation4.From = morningSkyColor5;
            stippledAnimation4.To = sunsetSkyColor5;
            stippledAnimation4.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation bgAnimation1 = new ColorAnimation();
            bgAnimation1.From = morningSkyColor1;
            bgAnimation1.To = sunsetSkyColor1;
            bgAnimation1.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation bgAnimation2 = new ColorAnimation();
            bgAnimation2.From = morningSkyColor2;
            bgAnimation2.To = sunsetSkyColor2;
            bgAnimation2.Duration = TimeSpan.FromSeconds(1.5);
            
            ColorAnimation bgAnimation3 = new ColorAnimation();
            bgAnimation3.From = morningSkyColor3;
            bgAnimation3.To = sunsetSkyColor3;
            bgAnimation3.Duration = TimeSpan.FromSeconds(1.5);
                        
            ColorAnimation bgAnimation4 = new ColorAnimation();
            bgAnimation4.From = morningSkyColor4;
            bgAnimation4.To = sunsetSkyColor4;
            bgAnimation4.Duration = TimeSpan.FromSeconds(1.5);
            
            ColorAnimation bgAnimation5 = new ColorAnimation();
            bgAnimation5.From = morningSkyColor5;
            bgAnimation5.To = sunsetSkyColor5;
            bgAnimation5.Duration = TimeSpan.FromSeconds(1.5);
            
            // need to freeze these animations before adding them to the storyboard
            Storyboard.SetTarget(bgAnimation1, SkyBackground1Brush);
            Storyboard.SetTarget(bgAnimation2, SkyBackground2Brush);
            Storyboard.SetTarget(bgAnimation3, SkyBackground3Brush);
            Storyboard.SetTarget(bgAnimation4, SkyBackground4Brush);
            Storyboard.SetTarget(bgAnimation5, SkyBackground5Brush);
            
            Storyboard.SetTarget(stippledAnimation1, brush1ToAnimate);
            Storyboard.SetTarget(stippledAnimation2, brush2ToAnimate);
            Storyboard.SetTarget(stippledAnimation3, brush3ToAnimate);
            Storyboard.SetTarget(stippledAnimation4, brush4ToAnimate);

            Storyboard.SetTargetProperty(bgAnimation1, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation2, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation3, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation4, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation5, new PropertyPath(SolidColorBrush.ColorProperty));
            
            Storyboard.SetTargetProperty(stippledAnimation1, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(stippledAnimation2, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(stippledAnimation3, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(stippledAnimation4, new PropertyPath(SolidColorBrush.ColorProperty));

            bgAnimation1.Freeze();
            bgAnimation2.Freeze();
            bgAnimation3.Freeze();
            bgAnimation4.Freeze();
            bgAnimation5.Freeze();
            
            stippledAnimation1.Freeze();
            stippledAnimation2.Freeze();
            stippledAnimation3.Freeze();
            stippledAnimation4.Freeze();

            Stippled1SunsetStoryBoard.Children.Add(stippledAnimation1);
            Stippled2SunsetStoryBoard.Children.Add(stippledAnimation2);
            Stippled3SunsetStoryBoard.Children.Add(stippledAnimation3);
            Stippled4SunsetStoryBoard.Children.Add(stippledAnimation4);

            SkyGradientSunsetStoryBoard1.Children.Add(bgAnimation1);
            SkyGradientSunsetStoryBoard2.Children.Add(bgAnimation2);
            SkyGradientSunsetStoryBoard3.Children.Add(bgAnimation3);
            SkyGradientSunsetStoryBoard4.Children.Add(bgAnimation4);
            SkyGradientSunsetStoryBoard5.Children.Add(bgAnimation5);
            
            Stippled1SunsetStoryBoard.Freeze();
            Stippled2SunsetStoryBoard.Freeze();
            Stippled3SunsetStoryBoard.Freeze();
            Stippled4SunsetStoryBoard.Freeze();

            SkyGradientSunsetStoryBoard1.Freeze();
            SkyGradientSunsetStoryBoard2.Freeze();
            SkyGradientSunsetStoryBoard3.Freeze();
            SkyGradientSunsetStoryBoard4.Freeze();
            SkyGradientSunsetStoryBoard5.Freeze();
        }

        private void TransitionFromDayToSunset()
        {
            timeCode = 1;
            RenderTimeBasedOnCalculation(true);

            //SkyGradientSunsetStoryBoard.Begin(this, true);
            SkyGradientSunsetStoryBoard1.Begin();
            SkyGradientSunsetStoryBoard2.Begin();
            SkyGradientSunsetStoryBoard3.Begin();
            SkyGradientSunsetStoryBoard4.Begin();
            SkyGradientSunsetStoryBoard5.Begin();

            //Stippled1SunsetStoryBoard.Begin(this, true);
            Stippled1SunsetStoryBoard.Begin();
            Stippled2SunsetStoryBoard.Begin();
            Stippled3SunsetStoryBoard.Begin();
            Stippled4SunsetStoryBoard.Begin();
        }

        private void ProvisionMorningAnimation(SolidColorBrush brush1ToAnimate, SolidColorBrush brush2ToAnimate, SolidColorBrush brush3ToAnimate, SolidColorBrush brush4ToAnimate)
        {
            // First cloud
            DoubleAnimation morningCloud1Animation = new DoubleAnimation();
            morningCloud1Animation.From = width + (786 * resolutionXMultiplier);
            morningCloud1Animation.To = -(786 * resolutionXMultiplier);
            morningCloud1Animation.SpeedRatio = 0.15;
            morningCloud1Animation.Duration = new Duration(TimeSpan.FromSeconds(10));
            morningCloud1Animation.RepeatBehavior = RepeatBehavior.Forever;
            
            Storyboard.SetTarget(morningCloud1Animation, Cloud1Rectangle);
            Storyboard.SetTargetProperty(morningCloud1Animation, new PropertyPath("(Canvas.Left)"));

            morningCloud1Animation.Freeze();
            Cloud1StoryBoard.Children.Add(morningCloud1Animation);
            Cloud1StoryBoard.Freeze();

            // second cloud
            DoubleAnimation morningCloud2Animation = new DoubleAnimation();
            morningCloud2Animation.From = width + (161 * resolutionXMultiplier);
            morningCloud2Animation.To = -(161 * resolutionXMultiplier);
            morningCloud2Animation.SpeedRatio = 0.25;
            morningCloud2Animation.Duration = new Duration(TimeSpan.FromSeconds(10));
            morningCloud2Animation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTarget(morningCloud2Animation, Cloud2Rectangle);
            Storyboard.SetTargetProperty(morningCloud2Animation, new PropertyPath("(Canvas.Left)"));

            morningCloud2Animation.Freeze();
            Cloud2StoryBoard.Children.Add(morningCloud2Animation);
            Cloud2StoryBoard.Freeze();
            
            // background gradients
            ColorAnimation bgAnimation1 = new ColorAnimation();
            bgAnimation1.From = dawnSkyColor1;
            bgAnimation1.To = morningSkyColor1;
            bgAnimation1.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation bgAnimation2 = new ColorAnimation();
            bgAnimation2.From = dawnSkyColor2;
            bgAnimation2.To = morningSkyColor2;
            bgAnimation2.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation bgAnimation3 = new ColorAnimation();
            bgAnimation3.From = dawnSkyColor3;
            bgAnimation3.To = morningSkyColor3;
            bgAnimation3.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation bgAnimation4 = new ColorAnimation();
            bgAnimation4.From = dawnSkyColor4;
            bgAnimation4.To = morningSkyColor4;
            bgAnimation4.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation bgAnimation5 = new ColorAnimation();
            bgAnimation5.From = dawnSkyColor5;
            bgAnimation5.To = morningSkyColor5;
            bgAnimation5.Duration = TimeSpan.FromSeconds(1.5);

            // stippled morning animations
            ColorAnimation stippledMorningAnimation1 = new ColorAnimation();
            stippledMorningAnimation1.From = dawnSkyColor2;
            stippledMorningAnimation1.To = morningSkyColor2;
            stippledMorningAnimation1.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation stippledMorningAnimation2 = new ColorAnimation();
            stippledMorningAnimation2.From = dawnSkyColor3;
            stippledMorningAnimation2.To = morningSkyColor3;
            stippledMorningAnimation2.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation stippledMorningAnimation3 = new ColorAnimation();
            stippledMorningAnimation3.From = dawnSkyColor4;
            stippledMorningAnimation3.To = morningSkyColor4;
            stippledMorningAnimation3.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation stippledMorningAnimation4 = new ColorAnimation();
            stippledMorningAnimation4.From = dawnSkyColor5;
            stippledMorningAnimation4.To = morningSkyColor5;
            stippledMorningAnimation4.Duration = TimeSpan.FromSeconds(1.5);

            Storyboard.SetTarget(bgAnimation1, SkyBackground1Brush);
            Storyboard.SetTarget(bgAnimation2, SkyBackground2Brush);
            Storyboard.SetTarget(bgAnimation3, SkyBackground3Brush);
            Storyboard.SetTarget(bgAnimation4, SkyBackground4Brush);
            Storyboard.SetTarget(bgAnimation5, SkyBackground5Brush);
                        
            Storyboard.SetTarget(stippledMorningAnimation1, brush1ToAnimate);
            Storyboard.SetTarget(stippledMorningAnimation2, brush2ToAnimate);
            Storyboard.SetTarget(stippledMorningAnimation3, brush3ToAnimate);
            Storyboard.SetTarget(stippledMorningAnimation4, brush4ToAnimate);

            Storyboard.SetTargetProperty(bgAnimation1, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation2, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation3, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation4, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation5, new PropertyPath(SolidColorBrush.ColorProperty));
            
            Storyboard.SetTargetProperty(stippledMorningAnimation1, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(stippledMorningAnimation2, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(stippledMorningAnimation3, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(stippledMorningAnimation4, new PropertyPath(SolidColorBrush.ColorProperty));

            bgAnimation1.Freeze();
            bgAnimation2.Freeze();
            bgAnimation3.Freeze();
            bgAnimation4.Freeze();
            bgAnimation5.Freeze();
            
            stippledMorningAnimation1.Freeze();
            stippledMorningAnimation2.Freeze();
            stippledMorningAnimation3.Freeze();
            stippledMorningAnimation4.Freeze();

            Stippled1MorningStoryBoard.Children.Add(stippledMorningAnimation1);
            Stippled2MorningStoryBoard.Children.Add(stippledMorningAnimation2);
            Stippled3MorningStoryBoard.Children.Add(stippledMorningAnimation3);
            Stippled4MorningStoryBoard.Children.Add(stippledMorningAnimation4);

            SkyGradientMorningStoryBoard1.Children.Add(bgAnimation1);
            SkyGradientMorningStoryBoard2.Children.Add(bgAnimation2);
            SkyGradientMorningStoryBoard3.Children.Add(bgAnimation3);
            SkyGradientMorningStoryBoard4.Children.Add(bgAnimation4);
            SkyGradientMorningStoryBoard5.Children.Add(bgAnimation5);
            
            Stippled1MorningStoryBoard.Freeze();
            Stippled2MorningStoryBoard.Freeze();
            Stippled3MorningStoryBoard.Freeze();
            Stippled4MorningStoryBoard.Freeze();

            SkyGradientMorningStoryBoard1.Freeze();
            SkyGradientMorningStoryBoard2.Freeze();
            SkyGradientMorningStoryBoard3.Freeze();
            SkyGradientMorningStoryBoard4.Freeze();
            SkyGradientMorningStoryBoard5.Freeze();
        }

        private void TransitionFromNightToDawn()
        {
            timeCode = 3;
            RenderTimeBasedOnCalculation(true);

            SkyGradientDawnStoryBoard1.Begin();
            SkyGradientDawnStoryBoard2.Begin();
            SkyGradientDawnStoryBoard3.Begin();
            SkyGradientDawnStoryBoard4.Begin();
            SkyGradientDawnStoryBoard5.Begin();

            Stippled1DawnStoryBoard.Begin();
            Stippled2DawnStoryBoard.Begin();
            Stippled3DawnStoryBoard.Begin();
            Stippled4DawnStoryBoard.Begin();
        }

        private void TransitionFromDawnToDay()
        {
            timeCode = 0;
            RenderTimeBasedOnCalculation(true);

            SkyGradientMorningStoryBoard1.Begin();
            SkyGradientMorningStoryBoard2.Begin();
            SkyGradientMorningStoryBoard3.Begin();
            SkyGradientMorningStoryBoard4.Begin();
            SkyGradientMorningStoryBoard5.Begin();

            Stippled1MorningStoryBoard.Begin();
            Stippled2MorningStoryBoard.Begin();
            Stippled3MorningStoryBoard.Begin();
            Stippled4MorningStoryBoard.Begin();

            /*
            if (Stippled1MorningStoryBoard.GetCurrentState() == ClockState.Active)
            {
            }
            */
        }

        private void ProvisionNightAnimation(SolidColorBrush brush1ToAnimate, SolidColorBrush brush2ToAnimate, SolidColorBrush brush3ToAnimate, SolidColorBrush brush4ToAnimate)
        {
            // moon animation for night
            DoubleAnimation moonAnimation = new DoubleAnimation();
            moonAnimation.From = width;
            moonAnimation.To = -(2021 * resolutionXMultiplier);
            moonAnimation.SpeedRatio = 0.15;
            moonAnimation.Duration = new Duration(TimeSpan.FromSeconds(10));
            moonAnimation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTarget(moonAnimation, Cloud4Rectangle);
            Storyboard.SetTargetProperty(moonAnimation, new PropertyPath("(Canvas.Left)"));
            moonAnimation.Freeze();

            Cloud4StoryBoard.Children.Add(moonAnimation);
            Cloud4StoryBoard.Freeze();

            // stippled layer color animations
            ColorAnimation stippledAnimation1 = new ColorAnimation();
            stippledAnimation1.From = sunsetSkyColor2;
            stippledAnimation1.To = nightSkyColor2;
            stippledAnimation1.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation stippledAnimation2 = new ColorAnimation();
            stippledAnimation2.From = sunsetSkyColor3;
            stippledAnimation2.To = nightSkyColor3;
            stippledAnimation2.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation stippledAnimation3 = new ColorAnimation();
            stippledAnimation3.From = sunsetSkyColor4;
            stippledAnimation3.To = nightSkyColor4;
            stippledAnimation3.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation stippledAnimation4 = new ColorAnimation();
            stippledAnimation4.From = sunsetSkyColor5;
            stippledAnimation4.To = nightSkyColor5;
            stippledAnimation4.Duration = TimeSpan.FromSeconds(1.5);

            // sky background layer color animations
            ColorAnimation bgAnimation1 = new ColorAnimation();
            bgAnimation1.From = sunsetSkyColor1;
            bgAnimation1.To = nightSkyColor1;
            bgAnimation1.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation bgAnimation2 = new ColorAnimation();
            bgAnimation2.From = sunsetSkyColor2;
            bgAnimation2.To = nightSkyColor2;
            bgAnimation2.Duration = TimeSpan.FromSeconds(1.5);
            
            ColorAnimation bgAnimation3 = new ColorAnimation();
            bgAnimation3.From = sunsetSkyColor3;
            bgAnimation3.To = nightSkyColor3;
            bgAnimation3.Duration = TimeSpan.FromSeconds(1.5);
                        
            ColorAnimation bgAnimation4 = new ColorAnimation();
            bgAnimation4.From = sunsetSkyColor4;
            bgAnimation4.To = nightSkyColor4;
            bgAnimation4.Duration = TimeSpan.FromSeconds(1.5);
                        
            ColorAnimation bgAnimation5 = new ColorAnimation();
            bgAnimation5.From = sunsetSkyColor5;
            bgAnimation5.To = nightSkyColor5;
            bgAnimation5.Duration = TimeSpan.FromSeconds(1.5);

            Storyboard.SetTarget(bgAnimation1, SkyBackground1Brush);
            Storyboard.SetTarget(bgAnimation2, SkyBackground2Brush);
            Storyboard.SetTarget(bgAnimation3, SkyBackground3Brush);
            Storyboard.SetTarget(bgAnimation4, SkyBackground4Brush);
            Storyboard.SetTarget(bgAnimation5, SkyBackground5Brush);

            Storyboard.SetTarget(stippledAnimation1, brush1ToAnimate);
            Storyboard.SetTarget(stippledAnimation2, brush2ToAnimate);
            Storyboard.SetTarget(stippledAnimation3, brush3ToAnimate);
            Storyboard.SetTarget(stippledAnimation4, brush4ToAnimate);

            Storyboard.SetTargetProperty(bgAnimation1, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation2, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation3, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation4, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation5, new PropertyPath(SolidColorBrush.ColorProperty));

            Storyboard.SetTargetProperty(stippledAnimation1, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(stippledAnimation2, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(stippledAnimation3, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(stippledAnimation4, new PropertyPath(SolidColorBrush.ColorProperty));
            
            bgAnimation1.Freeze();
            bgAnimation2.Freeze();
            bgAnimation3.Freeze();
            bgAnimation3.Freeze();
            bgAnimation5.Freeze();

            stippledAnimation1.Freeze();
            stippledAnimation2.Freeze();
            stippledAnimation3.Freeze();
            stippledAnimation4.Freeze();

            Stippled1NightStoryBoard.Children.Add(stippledAnimation1);
            Stippled2NightStoryBoard.Children.Add(stippledAnimation2);
            Stippled3NightStoryBoard.Children.Add(stippledAnimation3);
            Stippled4NightStoryBoard.Children.Add(stippledAnimation4);

            SkyGradientNightStoryBoard1.Children.Add(bgAnimation1);
            SkyGradientNightStoryBoard2.Children.Add(bgAnimation2);
            SkyGradientNightStoryBoard3.Children.Add(bgAnimation3);
            SkyGradientNightStoryBoard4.Children.Add(bgAnimation4);
            SkyGradientNightStoryBoard5.Children.Add(bgAnimation5);

            Stippled1NightStoryBoard.Freeze();
            Stippled2NightStoryBoard.Freeze();
            Stippled3NightStoryBoard.Freeze();
            Stippled4NightStoryBoard.Freeze();
            
            SkyGradientNightStoryBoard1.Freeze();
            SkyGradientNightStoryBoard2.Freeze();
            SkyGradientNightStoryBoard3.Freeze();
            SkyGradientNightStoryBoard4.Freeze();
            SkyGradientNightStoryBoard5.Freeze();
        }

        private void ProvisionDawnAnimation(SolidColorBrush brush1ToAnimate, SolidColorBrush brush2ToAnimate, SolidColorBrush brush3ToAnimate, SolidColorBrush brush4ToAnimate)
        {
            // moon animation for night
            DoubleAnimation moonAnimation = new DoubleAnimation();
            moonAnimation.From = width;
            moonAnimation.To = -(2021 * resolutionXMultiplier);
            moonAnimation.SpeedRatio = 0.15;
            moonAnimation.Duration = new Duration(TimeSpan.FromSeconds(10));
            moonAnimation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTarget(moonAnimation, Cloud5Rectangle);
            Storyboard.SetTargetProperty(moonAnimation, new PropertyPath("(Canvas.Left)"));
            moonAnimation.Freeze();

            Cloud5StoryBoard.Children.Add(moonAnimation);
            Cloud5StoryBoard.Freeze();

            // stippled layer color animations
            ColorAnimation stippledAnimation1 = new ColorAnimation();
            stippledAnimation1.From = nightSkyColor2;
            stippledAnimation1.To = dawnSkyColor2;
            stippledAnimation1.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation stippledAnimation2 = new ColorAnimation();
            stippledAnimation2.From = nightSkyColor3;
            stippledAnimation2.To = dawnSkyColor3;
            stippledAnimation2.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation stippledAnimation3 = new ColorAnimation();
            stippledAnimation3.From = nightSkyColor4;
            stippledAnimation3.To = dawnSkyColor4;
            stippledAnimation3.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation stippledAnimation4 = new ColorAnimation();
            stippledAnimation4.From = nightSkyColor5;
            stippledAnimation4.To = dawnSkyColor5;
            stippledAnimation4.Duration = TimeSpan.FromSeconds(1.5);

            // sky background layer color animations
            ColorAnimation bgAnimation1 = new ColorAnimation();
            bgAnimation1.From = nightSkyColor1;
            bgAnimation1.To = dawnSkyColor1;
            bgAnimation1.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation bgAnimation2 = new ColorAnimation();
            bgAnimation2.From = nightSkyColor2;
            bgAnimation2.To = dawnSkyColor2;
            bgAnimation2.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation bgAnimation3 = new ColorAnimation();
            bgAnimation3.From = nightSkyColor3;
            bgAnimation3.To = dawnSkyColor3;
            bgAnimation3.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation bgAnimation4 = new ColorAnimation();
            bgAnimation4.From = nightSkyColor4;
            bgAnimation4.To = dawnSkyColor4;
            bgAnimation4.Duration = TimeSpan.FromSeconds(1.5);

            ColorAnimation bgAnimation5 = new ColorAnimation();
            bgAnimation5.From = nightSkyColor5;
            bgAnimation5.To = dawnSkyColor5;
            bgAnimation5.Duration = TimeSpan.FromSeconds(1.5);

            Storyboard.SetTarget(bgAnimation1, SkyBackground1Brush);
            Storyboard.SetTarget(bgAnimation2, SkyBackground2Brush);
            Storyboard.SetTarget(bgAnimation3, SkyBackground3Brush);
            Storyboard.SetTarget(bgAnimation4, SkyBackground4Brush);
            Storyboard.SetTarget(bgAnimation5, SkyBackground5Brush);

            Storyboard.SetTarget(stippledAnimation1, brush1ToAnimate);
            Storyboard.SetTarget(stippledAnimation2, brush2ToAnimate);
            Storyboard.SetTarget(stippledAnimation3, brush3ToAnimate);
            Storyboard.SetTarget(stippledAnimation4, brush4ToAnimate);

            Storyboard.SetTargetProperty(bgAnimation1, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation2, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation3, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation4, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(bgAnimation5, new PropertyPath(SolidColorBrush.ColorProperty));

            Storyboard.SetTargetProperty(stippledAnimation1, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(stippledAnimation2, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(stippledAnimation3, new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard.SetTargetProperty(stippledAnimation4, new PropertyPath(SolidColorBrush.ColorProperty));

            bgAnimation1.Freeze();
            bgAnimation2.Freeze();
            bgAnimation3.Freeze();
            bgAnimation3.Freeze();
            bgAnimation5.Freeze();

            stippledAnimation1.Freeze();
            stippledAnimation2.Freeze();
            stippledAnimation3.Freeze();
            stippledAnimation4.Freeze();

            Stippled1DawnStoryBoard.Children.Add(stippledAnimation1);
            Stippled2DawnStoryBoard.Children.Add(stippledAnimation2);
            Stippled3DawnStoryBoard.Children.Add(stippledAnimation3);
            Stippled4DawnStoryBoard.Children.Add(stippledAnimation4);

            SkyGradientDawnStoryBoard1.Children.Add(bgAnimation1);
            SkyGradientDawnStoryBoard2.Children.Add(bgAnimation2);
            SkyGradientDawnStoryBoard3.Children.Add(bgAnimation3);
            SkyGradientDawnStoryBoard4.Children.Add(bgAnimation4);
            SkyGradientDawnStoryBoard5.Children.Add(bgAnimation5);

            Stippled1DawnStoryBoard.Freeze();
            Stippled2DawnStoryBoard.Freeze();
            Stippled3DawnStoryBoard.Freeze();
            Stippled4DawnStoryBoard.Freeze();

            SkyGradientDawnStoryBoard1.Freeze();
            SkyGradientDawnStoryBoard2.Freeze();
            SkyGradientDawnStoryBoard3.Freeze();
            SkyGradientDawnStoryBoard4.Freeze();
            SkyGradientDawnStoryBoard5.Freeze();
        }

        private void TransitionFromDayToNight()
        {
            timeCode = 2;
            RenderTimeBasedOnCalculation(true);

            SkyGradientNightStoryBoard1.Begin();
            SkyGradientNightStoryBoard2.Begin();
            SkyGradientNightStoryBoard3.Begin();
            SkyGradientNightStoryBoard4.Begin();
            SkyGradientNightStoryBoard5.Begin();

            Stippled1NightStoryBoard.Begin();
            Stippled2NightStoryBoard.Begin();
            Stippled3NightStoryBoard.Begin();
            Stippled4NightStoryBoard.Begin();
        }
                
        void ShowFrontEndStoryboard_Completed(object sender, EventArgs e)
        {
            // play music as it fades in
            if (music == true && challengerJoined == false)
            {
                VideoElement.Volume = 0;
                wmpVolume = 0;
                BigBlue.XAudio2Player.PlaySound(MusicSoundKey, null);
            }

            // continue counting time towards the screen saver when you return from the game
            if (screenSaverTimeInMinutes >= 1)
            {
                screenSaverTimer.Start();
            }

            if (frontendShown == false)
            {
                frontendShown = true;

                BigBlue.NativeMethods.ProvisionRawInputs(this, false);
                // you don't need this anymore
                //BigBlue.NativeMethods.RegisterInputDevices(this);

                mouseStopWatch.Start();

                // hide mouse cursor code
                if (hideMouseCursor == true)
                {
                    string cursorFileName = "HiddenCursor.cur";

                    IntPtr appstartCursor = BigBlue.NativeMethods.LoadCursorFromFile(cursorFileName);
                    bool setAppstartCursorSuccessfully = BigBlue.NativeMethods.SetSystemCursor(appstartCursor, BigBlue.NativeMethods.OCR_APPSTARTING);

                    IntPtr standardCursor = BigBlue.NativeMethods.LoadCursorFromFile(cursorFileName);
                    bool setStandardCursorSuccessfully = BigBlue.NativeMethods.SetSystemCursor(standardCursor, BigBlue.NativeMethods.OCR_NORMAL);

                    IntPtr hourglassCursor = BigBlue.NativeMethods.LoadCursorFromFile(cursorFileName);
                    bool setHourglassCursorSuccessfully = BigBlue.NativeMethods.SetSystemCursor(hourglassCursor, BigBlue.NativeMethods.OCR_WAIT);
                }
            }

            // start the video fade in stopwatch
            videoFadeInStopwatch.Start();
        }
        
        void StaticAfterExplosionStoryboard_Completed(object sender, EventArgs e)
        {
            shutdown = true;
            canThrowKnife = false;

            FightersRectangle.Fill = fightersAnimationFrames[timeCode][0];
            fighterCurrentFrame = 0;

            //Fighters.Viewbox = fighterRects[0];
            explosionTimer.Start();

            StopVideo();

            // since we won't be going back, we can just collapse all these elements
            VideoElement.Visibility = System.Windows.Visibility.Collapsed;
            SnapShotRectangle.Visibility = System.Windows.Visibility.Collapsed;
            SnapShotBackgroundRectangle.Visibility = System.Windows.Visibility.Collapsed;
            KOCountdown.Visibility = System.Windows.Visibility.Collapsed;
            //GameList.Visibility = System.Windows.Visibility.Collapsed;

            BigBlue.XAudio2Player.PlaySound("buildingexplosion", null);
                        
            // this is the best frame for the crowd
            foreach (Rectangle spectatorTile in spectatorTiles)
            {
                spectatorTile.Fill = spectatorAnimations[streetFighterEditionAnimation][timeCode][3];                   
                MakeCrowdFlee();
            }
            
            if (georgeState != RampageState.dead && georgeState != RampageState.falling && georgeState != RampageState.burning)
            {
                DestroyGeorge("suicide");
            }

            if (rtypeDestroyed == false && rtypeShipState != RtypeState.dead)
            {
                DestroyRtype();
            }
        }

        void RegularlyScheduledProgrammingStoryboard_Completed(object sender, EventArgs e)
        {
            KnifeStoryboard.Stop();
            shutdownStoryboard.Stop();

            StaticRectangle.Visibility = System.Windows.Visibility.Hidden;
            
            if (marqueeDisplay == true)
            {
                marqueeWindow.staticAnimating = false;
            
                marqueeWindow.SecondaryWindowStatic.Visibility = System.Windows.Visibility.Collapsed;
                marqueeWindow.DamndBackground.Visibility = System.Windows.Visibility.Collapsed;
                marqueeWindow.Damnd.Visibility = System.Windows.Visibility.Collapsed;

                marqueeWindow.damndAnimationTimer.Stop();
                marqueeWindow.damndAnimationTimer.Tick -= marqueeWindow.animateSecondaryDisplay;
                marqueeWindow.damndGloating = false;
            }

            if (flyerDisplay == true)
            {
                flyerWindow.staticAnimating = false;

                flyerWindow.SecondaryWindowStatic.Visibility = System.Windows.Visibility.Collapsed;
                flyerWindow.DamndBackground.Visibility = System.Windows.Visibility.Collapsed;
                flyerWindow.Damnd.Visibility = System.Windows.Visibility.Collapsed;

                flyerWindow.damndAnimationTimer.Stop();
                flyerWindow.damndAnimationTimer.Tick -= flyerWindow.animateSecondaryDisplay;
                flyerWindow.damndGloating = false;
            }

            if (instructionDisplay == true)
            {
                instructionWindow.staticAnimating = false;

                instructionWindow.SecondaryWindowStatic.Visibility = System.Windows.Visibility.Collapsed;
                instructionWindow.DamndBackground.Visibility = System.Windows.Visibility.Collapsed;
                instructionWindow.Damnd.Visibility = System.Windows.Visibility.Collapsed;

                instructionWindow.damndAnimationTimer.Stop();
                instructionWindow.damndAnimationTimer.Tick -= instructionWindow.animateSecondaryDisplay;
                instructionWindow.damndGloating = false;
            }

            //damndTimer.Stop();

            //damndGloating = false;

            IgniteRectangle.Visibility = System.Windows.Visibility.Hidden;
            HaggarBackgroundRectangle.Visibility = System.Windows.Visibility.Hidden;
            HaggarHeadRectangle.Visibility = System.Windows.Visibility.Hidden;
            HaggarForegroundRectangle.Visibility = System.Windows.Visibility.Hidden;
            HaggarCanvas.Visibility = System.Windows.Visibility.Hidden;

            if (roundInProgress == false && challengerIntroRunning == false)
            {
                GameList.Visibility = System.Windows.Visibility.Visible;
            }

            shutdownSequenceActivated = false;

            // there are two different scenarios:
            // 1) we return from an exit sequence when the video WASN'T playing
            // 2) we return from an exit sequence when the video WAS playing
            // if the video was playing, we'll just resume it
            if (isVideoPlaying == true)
            {
                // you have to do this after the shut down sequence has been defined as false, or it won't resume the video
                ResumeVideo();
            }
            else
            {
                // don't restart the video fade in if a versus round is going on
                if (roundInProgress == false)
                {
                    // if the video wasn't playing, then we're just going to restart the stopwatch
                    videoFadeInStopwatch.Reset();
                    videoFadeInStopwatch.Start();
                }
            }

            UpdateClock();
        }
        
        void ShutdownStoryboard_Completed(object sender, EventArgs e)
        {
            canThrowKnife = true;
            haggarInDanger = true;
            BigBlue.XAudio2Player.PlaySound("fuse", null);

            if (marqueeDisplay == true || flyerDisplay == true || instructionDisplay == true)
            {
                //damndTimer.Start();
                if (marqueeDisplay == true)
                {
                    marqueeWindow.SecondaryWindowStatic.Visibility = System.Windows.Visibility.Collapsed;
                    marqueeWindow.DamndBackground.Visibility = System.Windows.Visibility.Visible;
                    marqueeWindow.Damnd.Visibility = System.Windows.Visibility.Visible;

                    marqueeWindow.staticAnimating = false;
                    marqueeWindow.damndGloating = true;
                }

                if (flyerDisplay == true)
                {
                    flyerWindow.SecondaryWindowStatic.Visibility = System.Windows.Visibility.Collapsed;
                    flyerWindow.DamndBackground.Visibility = System.Windows.Visibility.Visible;
                    flyerWindow.Damnd.Visibility = System.Windows.Visibility.Visible;

                    flyerWindow.staticAnimating = false;
                    flyerWindow.damndGloating = true;
                }

                if (instructionDisplay == true)
                {
                    instructionWindow.SecondaryWindowStatic.Visibility = System.Windows.Visibility.Collapsed;
                    instructionWindow.DamndBackground.Visibility = System.Windows.Visibility.Visible;
                    instructionWindow.Damnd.Visibility = System.Windows.Visibility.Visible;

                    instructionWindow.staticAnimating = false;
                    instructionWindow.damndGloating = true;
                }
            }

            //damndGloating = true;

            IgniteRectangle.Visibility = System.Windows.Visibility.Visible;
            HaggarHeadRectangle.Visibility = System.Windows.Visibility.Visible;
            HaggarBackgroundRectangle.Visibility = System.Windows.Visibility.Visible;
            HaggarForegroundRectangle.Visibility = System.Windows.Visibility.Visible;
        }

        void KnifeAnimation_Completed(object sender, EventArgs e)
        {
            BigBlue.XAudio2Player.PlaySound("knife", null);
            
            if (IgniteRectangle.Opacity == 0)
            {
                HaggarHeadRectangle.Fill = haggarAnimationFrames[6];
                //HaggarImage.Viewbox = haggarRects[6];

                //KnifeStoryboard.Stop();
                //shutdownStoryboard.Begin();

                if (marqueeDisplay == true || flyerDisplay == true || instructionDisplay == true)
                {
                    if (marqueeDisplay == true)
                    {
                        marqueeWindow.SecondaryWindowStatic.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    if (flyerDisplay == true)
                    {
                        flyerWindow.SecondaryWindowStatic.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    if (instructionDisplay == true)
                    {
                        instructionWindow.SecondaryWindowStatic.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                
                regularlyScheduledProgrammingStoryboard.Begin();
            }    
        }
        
        

        DispatcherTimer timeCodeTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
        
        

        public MainWindow(string baseDirectory, XmlNode config)
        {
            try
            {
                this.hideWindowControls = true;

                InitializeComponent();
                
                this.Path = baseDirectory;
                this.ConfigNode = config;
                this.snapshotImageControl = SnapShotRectangle;
                this.textBlockListCanvas = GameList;
                this.videoMe = VideoElement;
                this.videoCanvas = Video;

                Application.Current.Deactivated += Current_Deactivated;
                
                //startFrontend();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Big Blue");
            }
        }
        
        private void Current_Deactivated(object sender, EventArgs e)
        {
            ReleaseAllInputs();
            RtypeDropWarp();
        }
        
        void MainWindow_Activated(object sender, EventArgs e)
        {
            if (mouseCursorTrapped == true)
            {
                TrapMouseCursor(FrontEndContainer);
            }
        }

        void TimeCodeTimer_Tick(object sender, EventArgs e)
        {
            // we're not going to turn on the screen saver if the system is shutting down
            // we're not going to turn on the screen saver if it's been set to less than a minute
            if (screenSaverTimeInMinutes >= 1 && screenSaver == false && shutdownSequenceActivated == false && shutdown == false)
            {
                CalculateScreenSaver();
            }

            if (dynamicTimeOfDay == true)
            {
                if (CalculateTimeCode() == true)
                {
                    if (timeCode == 0)
                    {
                        TransitionFromDawnToDay();

                    }
                    else if (timeCode == 1)
                    {
                        TransitionFromDayToSunset();
                    }
                    else if (timeCode == 2)
                    {
                        TransitionFromDayToNight();
                    }
                    else
                    {
                        TransitionFromNightToDawn();
                    }
                }
            }

            UpdateClock();
        }
                        
        private async void FightAnnouncement()
        {
            if (music == true)
            {
                BigBlue.XAudio2Player.PlaySound("guile", null);
            }

            BigBlue.XAudio2Player.PlaySound("fight", null);
            
            FightRectangle.Visibility = System.Windows.Visibility.Visible;

            awaitingAsync = true;

            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(1500);
            });

            awaitingAsync = false;

            FightAnnouncementCleanup();
        }
        
        private async void HereComesANewChallenger()
        {
            challengerJoined = true;
            challengerIntroRunning = true;
            newChallengerFrameToDisplay = 1;

            PauseFrontend();

            newChallengerTimer.Start();

            // try to play sound all playing sounds and play the new challenger ditty
            BigBlue.XAudio2Player.StopAllSounds();
            BigBlue.XAudio2Player.PlaySound("newchallenger", null);
                        
            // if the players have the menu open, we don't want to keep animating the new challenger text
            if (FrontEndContainer.Opacity != 1)
            {
                itsGoTime = false;
                ShowFrontEndStoryboard.SkipToFill();
            }

            //NewChallengerText.Opacity = 1;
            NewChallengerText.Visibility = System.Windows.Visibility.Visible;

            // only set it to be hidden if it was visible
            if (PressStartRectangle.Visibility == System.Windows.Visibility.Visible)
            {
                PressStartRectangle.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (BigBlue.XAudio2Player.Disabled)
            {
                awaitingAsync = true;

                await Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(2000);
                });

                awaitingAsync = false;

                NewChallengerCleanup();
            }
        }

        private void ShowVersusScreen()
        {
            //versusScreen = true;
            //NewChallengerText.Opacity = 0;
            NewChallengerText.Visibility = System.Windows.Visibility.Hidden;

            VsOverlay.Visibility = System.Windows.Visibility.Visible;
            ShowVersusStoryboard.Begin();
        }

        private void VersusHidden(object sender, EventArgs e)
        {
            VsOverlay.Visibility = System.Windows.Visibility.Collapsed;
            FightAnnouncement();
        }
                
        private async void VersusShown(object sender, EventArgs e)
        {
            // we want to completely wipe out the video when a versus game starts so that the transition back from the game is smoother
            if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Video != null)
            {
                StopVideo();
            }
            
            CleanUpLaserBeams();

            CalculateTime(true);

            ChooseRampageCharacter(false);

            fighterCombo = false;
            fighterFrameToDisplay = 0;

            blood1Storyboard.Resume(this);
            blood2Storyboard.Resume(this);
            blood3Storyboard.Resume(this);
            blood4Storyboard.Resume(this);

            FightersRectangle.Fill = fightersAnimationFrames[timeCode][0];
            fighterCurrentFrame = 0;
            //Fighters.Viewbox = fighterRects[0];

            BigBlue.XAudio2Player.PlaySound("versus", null);
            RespawnBuilding();
            SpawnRampageMonster(false);
            RespawnRtype(false);

            SetGameSnapshots(true);

            KOCountdown.Visibility = Visibility.Visible;
            KOCountdown.Opacity = 1;

            if (BigBlue.XAudio2Player.Disabled)
            {
                awaitingAsync = true;

                await Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(2000);
                });

                awaitingAsync = false;

                HideVersusScreen();
            }
        }

        private void HideVersusScreen()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                //versusScreen = false;
                
                HideVersusStoryboard.Begin();
            });
        }
        
        void RematchEnd(object sender, EventArgs e)
        {
            // why do this?
            //roundInProgress = false;
            fastMusic = false;
            celebratingVictory = false;
            FightAnnouncement();
        }

        private void StabilizeScreen()
        {
            screenShakeTimer.Stop();
            screenShakeTimer.Reset();

            screenShaking = false;
            FrontEndContainer.Margin = new Thickness(0, 0, 0, 0);
        }

        void RematchStart(object sender, EventArgs e)
        {
            //victoryTimer.Reset();
            // need to reset everything for a new match and then fade it in

            // recalculate time in case it changed
            CalculateTime(true);

            ChooseRampageCharacter(false);

            // hide the double KO rectangle if it was displayed
            DoubleKODecision.Visibility = System.Windows.Visibility.Hidden;
            
            // hide the draw game rectangle if it was displayed
            DrawGameRectangle.Visibility = System.Windows.Visibility.Hidden;
            
            // respawn the buildings, george, and r-9a
            RespawnBuilding();
            SpawnRampageMonster(false);
            RespawnRtype(false);

            // stop  the timer
            roundTimer.Stop();

            // if the George was around the lifebar and it was transparent when the fight ended, we need to reset its opacity to 1
            KOCountdown.Opacity = 1;

            // reset the countdown
            knockOutFrameToDisplay = 0;
            KOImage.Viewbox = koRects[0];
            FirstDigit.Fill = numbers["90"];
            SecondDigit.Fill = numbers["90"];
            roundDuration = 99;
            numberType = "0";
            
            StabilizeScreen();

            RematchEndStoryboard.Begin();
            //fadeEffect(1);
        }

        private void ResetFighters()
        {
            if (georgeState != RampageState.dead && georgeState != RampageState.falling && georgeState != RampageState.burning)
            {
                georgeState = RampageState.climbing;
            }

            FightersRectangle.Fill = fightersAnimationFrames[timeCode][0];
            fighterCurrentFrame = 0;
            //Fighters.Viewbox = fighterRects[0];
            fighterFrameToDisplay = 1;
        }

        internal override void ProvisionFrontendFadeAnimations(System.Windows.FrameworkElement feContainer)
        {
            DoubleAnimation fadeInAnimation = new DoubleAnimation();
            fadeInAnimation.From = 0;
            fadeInAnimation.To = 1;
            fadeInAnimation.SpeedRatio = 1;
            fadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            fadeInAnimation.AutoReverse = false;

            Storyboard.SetTarget(fadeInAnimation, feContainer);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));

            fadeInAnimation.Freeze();

            // fade out animation start
            DoubleAnimation fadeOutAnimation = new DoubleAnimation();
            fadeOutAnimation.From = 1;
            fadeOutAnimation.To = 0;
            fadeOutAnimation.SpeedRatio = 1;
            fadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            fadeOutAnimation.AutoReverse = false;

            Storyboard.SetTarget(fadeOutAnimation, feContainer);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));

            fadeOutAnimation.Freeze(); // fade out animation end

            DoubleAnimation fadeOutGameLaunchAnimation = new DoubleAnimation();
            fadeOutGameLaunchAnimation.From = 1;
            fadeOutGameLaunchAnimation.To = -1;
            fadeOutGameLaunchAnimation.SpeedRatio = 1;
            fadeOutGameLaunchAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));
            fadeOutGameLaunchAnimation.AutoReverse = false;
            fadeOutGameLaunchAnimation.Completed += SelectGame;

            Storyboard.SetTarget(fadeOutGameLaunchAnimation, feContainer);
            Storyboard.SetTargetProperty(fadeOutGameLaunchAnimation, new PropertyPath("Opacity"));

            fadeOutGameLaunchAnimation.Freeze(); // fade out animation end

            // wrap up show frontend storyboard
            ShowFrontEndStoryboard.Children.Add(fadeInAnimation);
            ShowFrontEndStoryboard.Completed += ShowFrontEndStoryboard_Completed;
            ShowFrontEndStoryboard.Freeze();

            // wrap up launch game storyboard
            LaunchGameStoryboard.Children.Add(fadeOutGameLaunchAnimation);
            //LaunchGameStoryboard.Completed += selectGame;
            LaunchGameStoryboard.Freeze();

            // wrap up rematch start storyboard
            RematchStartStoryboard.Children.Add(fadeOutAnimation);
            RematchStartStoryboard.Completed += RematchStart;
            RematchStartStoryboard.Freeze();

            // wrap up rematch end storyboard
            RematchEndStoryboard.Children.Add(fadeInAnimation);
            RematchEndStoryboard.Completed += RematchEnd;
            RematchEndStoryboard.Freeze();
        }

        private void SelectGame(object sender, EventArgs e)
        {
            try
            {
                ReleaseAllInputs();

                StopVideo();

                CompositionTarget.Rendering -= OnFrame;

                // stop counting time towards the screen saver when you start a game
                screenSaverTimer.Stop();

                if (freeForAll == true)
                {
                    RespawnRtype(false);
                    StabilizeScreen();
                }

                LaunchProgram(FrontEndContainer, directoryToLaunch, fileNameToLaunch, argumentsToLaunch, lt);
            }
            catch
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    BackToGameList();

                    awaitingAsync = true;
                    PlayLoseSound();
                });
            }
        }
        
        private void RespawnRtype(bool autoStart)
        {
            Two.Width = lifeBarWidth;

            if (rtypeShipState == RtypeState.flying)
            {
                RtypeFlyingStoryboard.Stop();
            }

            //rtypeImageBrush.Viewbox = rtypeRects[0];
            Rtype.Fill = rtypeAnimationFrames[timeCode][0];
            rtypeCurrentFrame = 0;
            //RtypeImage.Viewbox = rtypeRects[0];
            PlasmaImage.Viewbox = plasmaRects[0];
            Canvas.SetLeft(Rtype, -(rtypeWidth));
            Canvas.SetLeft(RtypePlasma, rtypePlasmaOffset);
            //Canvas.SetLeft(RtypeCollisionBox, -rtypeWidth);

            // reset the ship status to flying and make sure that the warp resource stopwatch isn't still running
            rtypeShipState = RtypeState.flying;
            warpStopWatch.Reset();

            rtypeDestroyed = false;
            rtypeFrameToDisplay = 0;

            if (autoStart == true)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    RtypeFlyingStoryboard.Begin();
                }, System.Windows.Threading.DispatcherPriority.Render, null);
            }
        }
        
        internal override void BackToGameList()
        {
            BigBlue.XAudio2Player.RestartAudioEngine();

            Dispatcher.Invoke((Action)delegate
            {
                CleanUpLaserBeams();

                CompositionTarget.Rendering += OnFrame;

                VideoElement.Volume = minimumVolume;

                FrontEndWindow.Activate();
                //FrontEndWindow.Topmost = true;

                itsGoTime = false;

                decisionMade = false;

                ResetFighters();

                // the round truly ends after a game has been selected and exited
                roundInProgress = false;

                CalculateTime(true);

                // choose a new rampage character
                ChooseRampageCharacter(true);

                if (freeForAll == true)
                {
                    SpawnRampageMonster(true);
                    RespawnRtype(true);
                }

                ResumeFrontend();

                RespawnBuilding();

                ShowFrontEndStoryboard.Begin();
                isVideoPlaying = false;

            }, System.Windows.Threading.DispatcherPriority.Render, null);
            
            
        }
        
        Rect knifeCollisionRect = new Rect();
        Rect fuseCollisionRect = new Rect();

        private void CalculateFuseCollision()
        {
            knifeCollisionRect.Y = Canvas.GetTop(KnifeRectangle);

            fuseCollisionRect.X = Canvas.GetLeft(IgniteCollisionBox);
            fuseCollisionRect.Y = Canvas.GetTop(IgniteCollisionBox);
            
            if (fuseCollisionRect.IntersectsWith(knifeCollisionRect) == true)
            {
                haggarInDanger = false;
                BigBlue.XAudio2Player.StopSound("fuse");
                
                IgniteRectangle.Opacity = 0;
            }
        }

        private void CalculatePlasmaCollisions()
        {

            if (rtypeDestroyed == false && rtypeShipState == RtypeState.warping && georgeState != RampageState.falling && georgeState != RampageState.burning && georgeState != RampageState.dead)
            {
                // define george's plasma collision box
                plasmaCollisionRect.X = Canvas.GetLeft(RtypePlasma);
                plasmaCollisionRect.Y = Canvas.GetTop(RtypePlasma);

                if (plasmaCollisionRect.IntersectsWith(georgeBodyCollisionRect) == true)
                {
                    DestroyGeorge("burnt");
                }
            }
        }

        double decalOffset = 0;

        Rect georgeBodyCollisionRect = new Rect();
        Rect currentLaserCollisionRect = new Rect();
        Rect plasmaCollisionRect = new Rect();
        
        private void CalculateLaserCollisions()
        {
            // we're not going to bother calculating the collisions unless r9-a fired some lasers
            int totalLasers = laserBeams.Count;

            if (totalLasers == 0)
            {
                return;
            }
            else
            {
                // we're going to queue up the indexies of the lasers that need to be deleted
                List<Guid> laserDeletionQueue = new List<Guid>();
                
                foreach (KeyValuePair<Guid, Rectangle> laserBeam in laserBeams)
                {
                    Rectangle laser = laserBeam.Value;

                    // get the laser's current position
                    double currentLaserBeamX = Canvas.GetLeft(laser);
                    double currentLaserBeamY = Canvas.GetTop(laser);
                    
                    //Rect currentLaserRect = new Rect(currentLaserBeamX, currentLaserBeamY, rtypeLaserWidth, rtypeLaserHeight);
                    currentLaserCollisionRect.X = currentLaserBeamX;
                    currentLaserCollisionRect.Y = currentLaserBeamY;

                    // if the laser intersects with the body collision rectangle, george's state isn't already falling, and george isn't already dead, then we register a shot as having hit george
                    if (currentLaserCollisionRect.IntersectsWith(georgeBodyCollisionRect) == true && georgeState != RampageState.falling && georgeState != RampageState.dead && georgeState != RampageState.burning)
                    {
                        laserDeletionQueue.Add(laserBeam.Key);
                        
                        // if the laser beam is lower than the collision box, we're going to subtract half the height of the decal from the position of the blood splat
                        // if the laser beam isn't lower than the collision box, we're going to add half the height of the decal from the position of the blood splat
                        if (currentLaserBeamY - georgeBodyCollisionRect.Y >= 0)
                        {
                            decalOffset = (currentLaserBeamY - currentRampageTopPosition) - damageDecalYCenter;
                        }
                        else
                        {
                            decalOffset = (currentLaserBeamY - currentRampageTopPosition) + damageDecalYCenter;
                        }

                        // set the position of the damage decal to the position that the laser was in when it hit george
                        Canvas.SetLeft(GeorgeDamageDecal, georgeBodyCollisionRect.X);

                        Canvas.SetTop(GeorgeDamageDecal, currentLaserBeamY);
                        
                        // make the decal visible
                        if (GeorgeDamageDecal.Visibility == System.Windows.Visibility.Hidden)
                        {
                            GeorgeDamageDecal.Visibility = System.Windows.Visibility.Visible;
                        }

                        // maybe pause the storyboard and set opacity of laser to zero here
                        DestroyGeorge("shot");
                    }
                    else
                    {
                        // if there wasn't a collision, we remove the laser from the canvas when it's off the screen
                        if (currentLaserBeamX >= width)
                        {
                            laserDeletionQueue.Add(laserBeam.Key);
                        }
                        else
                        {
                            // make the rtype lasers flicker
                            if (laser.Opacity == 1)
                            {
                                laser.Opacity = 0;
                            }
                            else
                            {
                                laser.Opacity = 1;
                            }
                        }
                    }
                }

                foreach (Guid id in laserDeletionQueue)
                {
                    // remove the storyboard for the laser
                    laserStoryboards.Remove(id);

                    // remove the rectangle from the canvas
                    FrontEndContainer.Children.Remove(laserBeams[id]);

                    // remove the rectangle from the rectangle list
                    laserBeams.Remove(id);
                }
            }
        }

        // make some kind of model that stores both the left and right arrays so you can grab the boxes by index

        Rect debrisLeftCollisionRect = new Rect();
        Rect debrisRightCollisionRect = new Rect();

        List<Rect> leftDebrisCollisionRects = new List<Rect>();
        List<Rect> rightDebrisCollisionRects = new List<Rect>();

        private void ProvisionDebrisCollisionRects()
        {
            
            Rect debrisCollisionRectLeft1 = new Rect();
            debrisCollisionRectLeft1.Width = 62 * resolutionXMultiplier;
            debrisCollisionRectLeft1.Height = 39 * resolutionXMultiplier;
            debrisCollisionRectLeft1.X = 37 * resolutionXMultiplier;
            debrisCollisionRectLeft1.Y = 13 * resolutionXMultiplier;

            leftDebrisCollisionRects.Add(debrisCollisionRectLeft1);

            Rect debrisCollisionRectRight1 = new Rect();
            debrisCollisionRectRight1.Width = 62 * resolutionXMultiplier;
            debrisCollisionRectRight1.Height = 39 * resolutionXMultiplier;
            debrisCollisionRectRight1.X = 121 * resolutionXMultiplier;
            debrisCollisionRectRight1.Y = 13 * resolutionXMultiplier;

            rightDebrisCollisionRects.Add(debrisCollisionRectRight1);
            

            Rect debrisCollisionRectLeft2 = new Rect();
            debrisCollisionRectLeft2.Width = 65 * resolutionXMultiplier;
            debrisCollisionRectLeft2.Height = 43 * resolutionXMultiplier;
            debrisCollisionRectLeft2.X = 30 * resolutionXMultiplier;
            debrisCollisionRectLeft2.Y = 13 * resolutionXMultiplier;

            leftDebrisCollisionRects.Add(debrisCollisionRectLeft2);

            Rect debrisCollisionRectRight2 = new Rect();
            debrisCollisionRectRight2.Width = 65 * resolutionXMultiplier;
            debrisCollisionRectRight2.Height = 43 * resolutionXMultiplier;
            debrisCollisionRectRight2.X = 125 * resolutionXMultiplier;
            debrisCollisionRectRight2.Y = 13 * resolutionXMultiplier;

            rightDebrisCollisionRects.Add(debrisCollisionRectRight2);

            Rect debrisCollisionRectLeft3 = new Rect();
            debrisCollisionRectLeft3.Width = 65 * resolutionXMultiplier;
            debrisCollisionRectLeft3.Height = 32 * resolutionXMultiplier;
            debrisCollisionRectLeft3.X = 20 * resolutionXMultiplier;
            debrisCollisionRectLeft3.Y = 22 * resolutionXMultiplier;

            leftDebrisCollisionRects.Add(debrisCollisionRectLeft3);

            Rect debrisCollisionRectRight3 = new Rect();
            debrisCollisionRectRight3.Width = 65 * resolutionXMultiplier;
            debrisCollisionRectRight3.Height = 32 * resolutionXMultiplier;
            debrisCollisionRectRight3.X = 135 * resolutionXMultiplier;
            debrisCollisionRectRight3.Y = 22 * resolutionXMultiplier;

            rightDebrisCollisionRects.Add(debrisCollisionRectRight3);

            Rect debrisCollisionRectLeft4 = new Rect();
            debrisCollisionRectLeft4.Width = 25 * resolutionXMultiplier;
            debrisCollisionRectLeft4.Height = 30 * resolutionXMultiplier;
            debrisCollisionRectLeft4.X = 23 * resolutionXMultiplier;
            debrisCollisionRectLeft4.Y = 24 * resolutionXMultiplier;

            leftDebrisCollisionRects.Add(debrisCollisionRectLeft4);

            Rect debrisCollisionRectRight4 = new Rect();
            debrisCollisionRectRight4.Width = 25 * resolutionXMultiplier;
            debrisCollisionRectRight4.Height = 30 * resolutionXMultiplier;
            debrisCollisionRectRight4.X = 172 * resolutionXMultiplier;
            debrisCollisionRectRight4.Y = 24 * resolutionXMultiplier;

            rightDebrisCollisionRects.Add(debrisCollisionRectRight4);
        }

        private bool DebrisIntersectsWithRtype(Rect rtypeRect)
        {
            bool debrisHit = false;

            // there's really no point in calculating the collision for the debris of the blocks that are below where the rtype ship flies
            //DrawCollision.Children.Clear();
            foreach (Rectangle debris in buildingDebris)
            {
                int debrisState = (int)debris.GetValue(FrameworkElement.TagProperty);
                
                if (debrisState > 0)
                {
                    //KeyTest.Text = debrisState.ToString();

                    // modify this by however much you want to offset it based on the frame of the animation
                    debrisLeftCollisionRect.X = debrisLeftPosition + leftDebrisCollisionRects[debrisState].X;
                    debrisRightCollisionRect.X = debrisLeftPosition + rightDebrisCollisionRects[debrisState].X;

                    double debrisTop = Canvas.GetTop(debris) + leftDebrisCollisionRects[debrisState].Y;

                    // modify this by however much you want to offset it based on the frame of animation
                    debrisLeftCollisionRect.Y = debrisTop;
                    debrisRightCollisionRect.Y = debrisTop;
                                        
                    debrisLeftCollisionRect.Width = leftDebrisCollisionRects[debrisState].Width;
                    debrisLeftCollisionRect.Height = leftDebrisCollisionRects[debrisState].Height;

                    debrisRightCollisionRect.Width = rightDebrisCollisionRects[debrisState].Width;
                    debrisRightCollisionRect.Height = rightDebrisCollisionRects[debrisState].Height;

                    // going to have to set the width and height of the collision boxes here too
                    /*
                    Rectangle box = new Rectangle();
                    box.Width = debrisLeftCollisionRect.Width;
                    box.Height = debrisLeftCollisionRect.Height;
                    box.Fill = blueBrush;
                    box.Opacity = 0.5;

                    Canvas.SetLeft(box, debrisLeftCollisionRect.X);
                    Canvas.SetTop(box, debrisLeftCollisionRect.Y);

                    Rectangle box2 = new Rectangle();
                    box2.Width = debrisRightCollisionRect.Width;
                    box2.Height = debrisRightCollisionRect.Height;
                    box2.Fill = blueBrush;
                    box2.Opacity = 0.5;

                    Canvas.SetLeft(box2, debrisRightCollisionRect.X);
                    Canvas.SetTop(box2, debrisRightCollisionRect.Y);

                    DrawCollision.Children.Add(box);

                    DrawCollision.Children.Add(box2);
                    */                    

                    // check against both boxes instead of just one
                    if (rtypeRect.IntersectsWith(debrisLeftCollisionRect) == true || rtypeRect.IntersectsWith(debrisRightCollisionRect))
                    {
                        debrisHit = true;
                        break;
                    }
                }
            }

            return debrisHit;
        }
        
        Storyboard flamingStoryboard;

        private void ProvisionRampageBurningAnimation()
        {
            // Create a MatrixTransform. This transform 
            // will be used to move the button.
            MatrixTransform flamingMatrixTransform = new MatrixTransform();
            RampageMonsterFlameRectangle.RenderTransform = flamingMatrixTransform;

            // Register the transform's name with the page 
            // so that it can be targeted by a Storyboard. 
            this.RegisterName("FlamingMatrixTransform", flamingMatrixTransform);
        }

        private void BurnRampageMonster()
        {
            double rampageLeft = Canvas.GetLeft(RampageMonsterRectangle);

            Canvas.SetLeft(RampageMonsterFlameRectangle, rampageLeft);
            Canvas.SetTop(RampageMonsterFlameRectangle, currentRampageTopPosition);
            //RampageMonsterFlameRectangle.Opacity = 1;
            RampageMonsterFlameRectangle.Visibility = System.Windows.Visibility.Visible;

            Point startingPoint = new Point(0, 0);
            // Create the animation path.
            PathGeometry flamingAnimationPath = new PathGeometry();
            PathFigure flamingPathFigure = new PathFigure();
            flamingPathFigure.StartPoint = startingPoint;
            PolyBezierSegment flamingPathBezierSegment = new PolyBezierSegment();

            double targetPosition = height - (currentRampageTopPosition + (293 * resolutionXMultiplier));
            double initialRisingUpAmount = -220 * resolutionXMultiplier;

            Point flamePoint1 = new Point(-60 * resolutionXMultiplier, initialRisingUpAmount);
            Point flamePoint2 = new Point(-200 * resolutionXMultiplier, 50 * resolutionXMultiplier);
            Point flamePoint3 = new Point(-180 * resolutionXMultiplier, targetPosition);
            
            //double dropDistance = targetPosition + initialRisingUpAmount;
            //double dropSpeedMultiplier = height / dropDistance;



            // 70 left, 190 up
            flamingPathBezierSegment.Points.Add(flamePoint1);
            // 210 left, 100 up
            flamingPathBezierSegment.Points.Add(flamePoint2);
            // 190 left, 700 down
            flamingPathBezierSegment.Points.Add(flamePoint3);

            flamingPathFigure.Segments.Add(flamingPathBezierSegment);
            flamingAnimationPath.Figures.Add(flamingPathFigure);

            // Freeze the PathGeometry for performance benefits.
            //flamingAnimationPath.Freeze();

            // Create a MatrixAnimationUsingPath to move the 
            // button along the path by animating 
            // its MatrixTransform.
            MatrixAnimationUsingPath flamingMatrixAnimation =
                new MatrixAnimationUsingPath();
            flamingMatrixAnimation.PathGeometry = flamingAnimationPath;
            flamingMatrixAnimation.SpeedRatio = verticalAnimationSpeedRatio;
            flamingMatrixAnimation.Duration = TimeSpan.FromSeconds(1.25);
            //blood1MatrixAnimation.DoesRotateWithTangent = true;
            //blood1MatrixAnimation.RepeatBehavior = RepeatBehavior.Forever;

            // Set the animation to target the Matrix property 
            // of the MatrixTransform named "ButtonMatrixTransform".
            Storyboard.SetTargetName(flamingMatrixAnimation, "FlamingMatrixTransform");
            Storyboard.SetTargetProperty(flamingMatrixAnimation,
                new PropertyPath(MatrixTransform.MatrixProperty));

            flamingMatrixAnimation.Freeze();

            // Create a Storyboard to contain and apply the animation.
            flamingStoryboard = new Storyboard();
            
            flamingStoryboard.Children.Add(flamingMatrixAnimation);
            
            flamingStoryboard.Completed += FlamingStoryboard_Completed;

            flamingStoryboard.Freeze();
            flamingStoryboard.Begin(this, true);
        }

        void FlamingStoryboard_Completed(object sender, EventArgs e)
        {
            georgeState = RampageState.dead;
            screenShaking = true;
            PlayGeorgeCrash();
            screenShakeTimer.Start();
            rampageFrameToDisplay = 0;
        }

        private const double BUILDING_FALLING_DURATION = 4;

        private void DropGeorge()
        {
            double targetPosition = height + (293 * resolutionXMultiplier);
            double dropDistance = targetPosition - currentRampageTopPosition;
            double dropSpeedMultiplier = height / dropDistance;

            georgeFallingAnimation.From = currentRampageTopPosition;
            georgeFallingAnimation.To = targetPosition;
            georgeFallingAnimation.SpeedRatio = verticalAnimationSpeedRatio * dropSpeedMultiplier;
            georgeFallingAnimation.Duration = new Duration(TimeSpan.FromSeconds(BUILDING_FALLING_DURATION));
            georgeFallingAnimation.AutoReverse = false;

            GeorgeFallingStoryBoard.Children.Add(georgeFallingAnimation);

            Storyboard.SetTarget(georgeFallingAnimation, RampageMonsterRectangle);

            // Set the attached properties of Canvas.Left and Canvas.Top
            // to be the target properties of the two respective DoubleAnimations.
            Storyboard.SetTargetProperty(georgeFallingAnimation, new PropertyPath("(Canvas.Top)"));

            // Begin the animation.
            GeorgeFallingStoryBoard.Begin();
        }

        private void DestroyGeorge(string mannerOfDeath)
        {
            // the fighters should stop punching when george is killed, because Player 1 should no longer have control
            fighterCombo = false;
            fighterAttacking = false;

            One.Width = 0;

            if (mannerOfDeath == "suicide")
            {
                // set george's state to falling
                georgeState = RampageState.falling;

                // pause the climbing animation 
                GeorgeClimbingStoryBoard.Pause();

                // reset the animation timer
                rampageElapsedMilliseconds = 0;
                //georgeTimer.Reset();
                //georgeTimer.Start();

                PlayRampageSlipping();

                //RampageImage.Viewbox = georgeRects[9];
                RampageMonsterRectangle.Fill = rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][9];
                rampageCurrentFrame = 9;
                rampageFrameToDisplay = 10;
            }
            
            if (mannerOfDeath == "shot")
            {
                // set george's state to falling
                georgeState = RampageState.falling;

                // stop the climbing animation
                GeorgeClimbingStoryBoard.Stop();
                DropGeorge();
                //RampageImage.Viewbox = georgeRects[10];
                RampageMonsterRectangle.Fill = rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][10];
                rampageCurrentFrame = 10;

                // play the scream sound effect for george's death
                PlayGeorgeScream();
            }

            if (mannerOfDeath == "burnt")
            {
                georgeState = RampageState.burning;

                // stop the climbing animation
                GeorgeClimbingStoryBoard.Stop();
                BurnRampageMonster();
                
                rampageFrameToDisplay = 6;

                // play the slap sound
                PlayFighterPunchSound();
            }
        }

        private async void DestroyRtype()
        {
            // flag the rtype ship as destroyed
            rtypeDestroyed = true;
                        
            PlayExplosionSound();

            // stop the rtype animation storyboard
            RtypeFlyingStoryboard.Pause();
            //RtypeFlyingStoryboard.Stop();
            // reset the rtype ship off the screen to the left
            //Canvas.SetLeft(RtypeCollisionBox, -(rtypeWidth));
            
            Two.Width = 0;

            if (BigBlue.XAudio2Player.Disabled)
            {
                awaitingAsync = true;

                await Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(1000);
                });

                awaitingAsync = false;

                RtypeExplosionComplete();
            }
        }
        
        private void RespawnUi()
        {
            roundInProgress = false;
            fastMusic = false;

            // if there was a video playing; need to check
            if (frontendLists[selectedListGuid].ListItems[selectedListItemIndex].Video != null)
            {
                videoFadeInStopwatch.Start();
            }

            // stop  the timer
            roundTimer.Stop();

            // hide the energy bars
            KOCountdown.Visibility = Visibility.Collapsed;
            KOCountdown.Opacity = 0;
            
            // show the gamelist again
            if (shutdownSequenceActivated == false)
            {
                GameList.Visibility = System.Windows.Visibility.Visible;
            }

            if (shutdownSequenceActivated == false)
            {
                SetGameSnapshots(false);
            }
            else
            {
                if (haggarInDanger)
                {
                    // if the shutdown sequence was started during the victory celebration, we're going to quietly reset the snapshots here
                    SetGameSnapshots(false);   
                }
            }
            
            // reset the countdown
            knockOutFrameToDisplay = 0;
            KOImage.Viewbox = koRects[0];
            FirstDigit.Fill = numbers["90"];
            SecondDigit.Fill = numbers["90"];
            roundDuration = 99;
            numberType = "0";
        }

        Rect rtypeCollisionRect = new Rect();
        
        private void CalculateRtypeCollision()
        {
            // don't bother calculating the collision if the rtype ship is destroyed
            if (rtypeDestroyed == false)
            {
                rtypeCollisionRect.X = currentRtypeLeftPosition;
                
                // if debris hits the rtype ship OR
                // if george is punching to the left and the rtype rectangle interects with his fist, register the hit OR
                // if george is climbing up and punches to the right and the rtype rectangle intersects with his second fist rectangle, register the hit OR
                // if george is climbing down and punches to the right and the rtype rectangle intersects with his third fist rectangle, register the hit
                //if (debrisIntersectsWithRtype(rtypeRect) == true || (RampageImage.Viewbox == georgeRects[4] && rtypeRect.IntersectsWith(georgePunch1Rect) == true) || (RampageImage.Viewbox == georgeRects[5] && rtypeRect.IntersectsWith(georgePunch2Rect) == true) || (RampageImage.Viewbox == georgeRects[6] && rtypeRect.IntersectsWith(georgePunch3Rect) == true))
                if (DebrisIntersectsWithRtype(rtypeCollisionRect) == true || (RampageMonsterRectangle.Fill == rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][4] && rtypeCollisionRect.IntersectsWith(georgeCollisionRect1) == true) || (RampageMonsterRectangle.Fill == rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][5] && rtypeCollisionRect.IntersectsWith(georgeCollisionRect2) == true) || (RampageMonsterRectangle.Fill == rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][6] && rtypeCollisionRect.IntersectsWith(georgeCollisionRect3) == true))
                {
                    DestroyRtype();

                    return;
                }
            }
        }

        private void ColorBuildingObjects()
        {
            int totalRightBlocks = buildingBlocksRight.Count() - 1;

            for (int i = 0; i < numberOfBuildingChunks; i++)
            {
                Rectangle block = buildingBlocks[i];
                Rectangle debris = buildingDebris[i];
                int debrisProperty = (int)debris.GetValue(FrameworkElement.TagProperty);
                int blockProperty = (int)block.GetValue(FrameworkElement.TagProperty);
                
                bool colorRightBlock = false;

                if (shutdown == true && totalRightBlocks >= i)
                {
                    colorRightBlock = true;
                }
                
                // this is getting -1 at some point
                debris.Fill = buildingDebrisAnimationFrames[timeCode][debrisProperty];
                block.Fill = buildingBlockAnimationFrames[timeCode][blockProperty];
                        
                if (colorRightBlock == true)
                {
                    buildingBlocksRight[i].Fill = buildingBlockAnimationFrames[timeCode][1];
                }
            }
        }

        // pristine = 0
        // damaged = 1
        // trucked = 2

        double debrisLeftPosition = 0;

        private void AddBuildingObjects()
        {
            double availableHeightForBuildingChunks = height - (252 * resolutionXMultiplier);

            numberOfBuildingChunks = Math.Ceiling(availableHeightForBuildingChunks / chunkHeight);

            chunksToCheck = Convert.ToInt32(numberOfBuildingChunks) - 1;

            int chunkCount = Convert.ToInt32(numberOfBuildingChunks);

            buildingDebris = new List<Rectangle>(chunkCount);
            buildingBlocks = new List<Rectangle>(chunkCount);
            buildingBlockDecals = new List<Rectangle>(chunkCount);
            
            double chunkLeftPosition = width - (totalBuildingsWidth * resolutionXMultiplier);
            double chunkWidth = 124 * resolutionXMultiplier;

            buildingCollisionBox.Width = chunkWidth;
            buildingCollisionBox.Height = chunkHeight;
            
            //debrisLeftCollisionRect.Width = 220 * resolutionXMultiplier;
            //debrisLeftCollisionRect.Height = 72 * resolutionXMultiplier;

            for (int i = 0; i < numberOfBuildingChunks; i++)
            {
                //Stopwatch debrisTimer = new Stopwatch();
                debrisElapsedMilliseconds.Add(0);
                //debrisTimers.Add(debrisTimer);

                Rectangle buildingBlock = new Rectangle();
                buildingBlock.IsHitTestVisible = false;
                buildingBlock.Width = chunkWidth;
                buildingBlock.Height = chunkHeight;
                buildingBlock.Fill = buildingBlockAnimationFrames[timeCode][0];
                buildingBlock.SetValue(FrameworkElement.TagProperty, 0);
                // set its default condition to "pristine"

                if (integerMultiplier == true)
                {
                    RenderOptions.SetBitmapScalingMode(buildingBlock, BitmapScalingMode.NearestNeighbor);
                }

                Canvas.SetLeft(buildingBlock, chunkLeftPosition);

                double chunkTop = chunkHeight * i;
                Canvas.SetTop(buildingBlock, chunkTop);

                FrontEndContainer.Children.Add(buildingBlock);

                buildingBlocks.Add(buildingBlock);

                Rectangle buildingBlockDecal = new Rectangle();
                buildingBlockDecal.IsHitTestVisible = false;
                buildingBlockDecal.Width = chunkWidth;
                buildingBlockDecal.Height = chunkHeight;
                buildingBlockDecal.SetValue(FrameworkElement.TagProperty, 0);

                if (integerMultiplier == true)
                {
                    RenderOptions.SetBitmapScalingMode(buildingBlockDecal, BitmapScalingMode.NearestNeighbor);
                }
                /*
                if (i % 2 == 0)
                {
                    buildingBlockDecal.Fill = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    buildingBlockDecal.Fill = new SolidColorBrush(Colors.Blue);
                }
                 */

                Canvas.SetLeft(buildingBlockDecal, chunkLeftPosition);

                Canvas.SetTop(buildingBlockDecal, chunkTop);

                FrontEndContainer.Children.Add(buildingBlockDecal);
                buildingBlockDecals.Add(buildingBlockDecal);

                Rectangle debrisRect = new Rectangle();
                debrisRect.IsHitTestVisible = false;

                debrisRect.Width = 220 * resolutionXMultiplier;
                debrisRect.Height = 72 * resolutionXMultiplier;
                
                debrisRect.Fill = buildingDebrisAnimationFrames[timeCode][0];

                //debrisRect.SetValue(FrameworkElement.TagProperty, -1);
                // experimental?
                debrisRect.SetValue(FrameworkElement.TagProperty, 0);

                //debrisRect.Opacity = 0;
                debrisRect.Visibility = System.Windows.Visibility.Hidden;

                if (integerMultiplier == true)
                {
                    RenderOptions.SetBitmapScalingMode(debrisRect, BitmapScalingMode.NearestNeighbor);
                }

                if (portraitModeIndex == 1)
                {
                    debrisLeftPosition = width - (1114 * resolutionXMultiplier);
                }
                else
                {
                    debrisLeftPosition = width - (1255 * resolutionXMultiplier);
                }

                Canvas.SetLeft(debrisRect, debrisLeftPosition);
                //debrisLeftCollisionRect.X = debrisLeftPosition;

                ProvisionDebrisCollisionRects();

                Canvas.SetTop(debrisRect, chunkTop + (26 * resolutionXMultiplier));
                Canvas.SetZIndex(debrisRect, 52);

                FrontEndContainer.Children.Add(debrisRect);

                RenderOptions.SetEdgeMode(debrisRect, EdgeMode.Aliased);

                buildingDebris.Add(debrisRect);

                // create the falling animations for these debris

                Storyboard debrisStoryboard = new Storyboard();
                // use to add debrisStoryboards here

                DoubleAnimation debrisAnimation = new DoubleAnimation();

                // chunkTop + (26 * resolutionXMultiplier);
                debrisAnimation.From = chunkTop;
                debrisAnimation.To = height + chunkTop;
                debrisAnimation.SpeedRatio = verticalAnimationSpeedRatio;
                debrisAnimation.Duration = new Duration(TimeSpan.FromSeconds(BUILDING_FALLING_DURATION));
                debrisAnimation.AutoReverse = false;

                Storyboard.SetTarget(debrisAnimation, buildingDebris[i]);

                // Set the attached properties of Canvas.Left and Canvas.Top
                // to be the target properties of the two respective DoubleAnimations.
                Storyboard.SetTargetProperty(debrisAnimation, new PropertyPath("(Canvas.Top)"));

                debrisAnimation.Freeze();

                //debrisAnimations.Add(debrisAnimation);

                debrisStoryboard.Children.Add(debrisAnimation);

                debrisStoryboard.Freeze();
                debrisStoryboards.Add(debrisStoryboard);
            }

            Canvas.SetZIndex(GeorgeDamageDecal, 51);
            Canvas.SetZIndex(RtypePlasma, 51);
            Canvas.SetZIndex(Rtype, 52);
        }

        private void AnimateNewChallengerText()
        {
            if (newChallengerTimer.ElapsedMilliseconds > 150)
            {
                NewChallengerImage.Viewbox = newChallengerRects[newChallengerFrameToDisplay];

                if (newChallengerFrameToDisplay == 0)
                {
                    newChallengerFrameToDisplay = 1;
                }
                else
                {
                    newChallengerFrameToDisplay = 0;
                }

                newChallengerTimer.Reset();
                newChallengerTimer.Start();
            }
        }

        private void Shake()
        {
            int rn = r.Next(screenShakeStartRange, screenShakeEndRange);
            FrontEndContainer.Margin = new Thickness(rn, rn, 0, 0);
        }

        private void ShakeScreen()
        {
            if (screenShaking == true)
            {
                if (screenShakeTimer.ElapsedMilliseconds <= 1000)
                {
                    Shake();
                }
                else
                {
                    StabilizeScreen();

                    if (freeForAll == true)
                    {
                        if (shutdown == false)
                        {
                            ChooseRampageCharacter(true);
                            SpawnRampageMonster(true);
                        }
                    }
                    else
                    {
                        if (decidingWinner == false)
                        {
                            decidingWinner = true;
                            // start the decision timer
                            decisionTimer.Start();
                        }
                    }
                }
            }
        }
        
        private void AnimateGeorge()
        {
            rampageElapsedMilliseconds = rampageElapsedMilliseconds + elapsedMilliseconds;

            if (georgeState != RampageState.dead)
            {
                if (georgeState == RampageState.falling)
                {
                    if (rampageElapsedMilliseconds > 1000)
                    //if (georgeTimer.ElapsedMilliseconds > 1000)
                    {
                        // update this
                        rampageElapsedMilliseconds = 0;

                        //georgeTimer.Reset();
                        //georgeTimer.Start();

                        if (rampageFrameToDisplay == 9 || rampageFrameToDisplay == 10)
                        {
                            if (rampageFrameToDisplay == 9)
                            {
                                rampageFrameToDisplay = 10;
                            }

                            //if (georgeFrameToDisplay == 10 && RampageImage.Viewbox != georgeRects[10])
                            if (rampageFrameToDisplay == 10 && RampageMonsterRectangle.Fill != rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][10])
                            {
                                GeorgeClimbingStoryBoard.Stop();
                                DropGeorge();
                            }

                            //RampageImage.Viewbox = georgeRects[georgeFrameToDisplay];
                            RampageMonsterRectangle.Fill = rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][rampageFrameToDisplay];
                            rampageCurrentFrame = rampageFrameToDisplay;
                        }
                    }
                }
                else if (georgeState == RampageState.burning)
                {
                    if (rampageElapsedMilliseconds > 16)
                    //if (georgeTimer.ElapsedMilliseconds > 16)
                    {
                        rampageElapsedMilliseconds = 0;
                        //georgeTimer.Reset();
                        //georgeTimer.Start();

                        if (rampageFrameToDisplay == 1)
                        {
                            rampageFrameToDisplay = 0;
                        }
                        else
                        {
                            rampageFrameToDisplay = 1;
                        }

                        RampageFlameImage.Viewbox = flamingRects[rampageFrameToDisplay];
                    }
                }
                else
                {
                    if (rampageElapsedMilliseconds > 150)
                    //if (georgeTimer.ElapsedMilliseconds > 150)
                    {
                        rampageElapsedMilliseconds = 0;
                        //georgeTimer.Reset();
                        //georgeTimer.Start();

                        GeorgeClimbingStoryBoard.Resume();
                        
                        // play the next frame of animation here depending on what george is doing
                        //RampageImage.Viewbox = georgeRects[georgeFrameToDisplay];
                        RampageMonsterRectangle.Fill = rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][rampageFrameToDisplay];
                        rampageCurrentFrame = rampageFrameToDisplay;

                        switch (rampageFrameToDisplay)
                        {
                            case 0:
                            case 2:
                                if (climbingUp == true)
                                {
                                    rampageFrameToDisplay = 1;
                                }
                                else
                                {
                                    rampageFrameToDisplay = 3;
                                }
                                break;
                            case 1:
                            case 3:
                            case 8:
                                if (climbingUp == true)
                                {
                                    rampageFrameToDisplay = 0;
                                }
                                else
                                {
                                    rampageFrameToDisplay = 2;
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void AnimateIgnite()
        {
            igniteElapsedMilliseconds = igniteElapsedMilliseconds + elapsedMilliseconds;

            //if (igniteTimer.ElapsedMilliseconds > 50)
            if (igniteElapsedMilliseconds > 50)
            {
                IgniteRectangle.Fill = igniteImageBrushes[igniteFrameToDisplay];

                igniteElapsedMilliseconds = 0;
                //igniteTimer.Stop();
                //igniteTimer.Reset();
                //igniteTimer.Start();

                if (igniteFrameToDisplay == 0)
                {
                    igniteFrameToDisplay = 1;
                }
                else
                {
                    igniteFrameToDisplay = 0;
                }
            }
        }
                
        private void AnimateStatic()
        {
            staticElapsedMilliseconds = staticElapsedMilliseconds + elapsedMilliseconds;

            //if (staticTimer.ElapsedMilliseconds > 100)
            if (staticElapsedMilliseconds > 100)
            {
                if (staticFrameToDisplay == 0)
                {
                    staticFrameToDisplay = 1;
                }
                else
                {
                    staticFrameToDisplay = 0;
                }

                staticElapsedMilliseconds = 0;

                StaticRectangle.Fill = staticBrushes[staticFrameToDisplay];

                // if marquee is enabled
                if (marqueeDisplay == true)
                {
                    // if invisible or collapsed
                    if (marqueeWindow.SecondaryWindowStatic.Visibility == System.Windows.Visibility.Collapsed && StaticRectangle.Visibility == System.Windows.Visibility.Visible)
                    {
                        marqueeWindow.SecondaryWindowStatic.Visibility = System.Windows.Visibility.Visible;
                        marqueeWindow.staticAnimating = true;
                    }
                }

                if (flyerDisplay == true)
                {
                    // if invisible or collapsed
                    if (flyerWindow.SecondaryWindowStatic.Visibility == System.Windows.Visibility.Collapsed && StaticRectangle.Visibility == System.Windows.Visibility.Visible)
                    {
                        flyerWindow.SecondaryWindowStatic.Visibility = System.Windows.Visibility.Visible;
                        flyerWindow.staticAnimating = true;
                    }
                }

                if (instructionDisplay == true)
                {
                    // if invisible or collapsed
                    if (instructionWindow.SecondaryWindowStatic.Visibility == System.Windows.Visibility.Collapsed && StaticRectangle.Visibility == System.Windows.Visibility.Visible)
                    {
                        instructionWindow.SecondaryWindowStatic.Visibility = System.Windows.Visibility.Visible;
                        instructionWindow.staticAnimating = true;
                    }
                }
            }

            //staticTimer.Stop();
            //staticTimer.Reset();
            //staticTimer.Start();
        }

        

        private void AddRightChunk(double chunkTop)
        {
            Rectangle rect = new Rectangle();
            rect.IsHitTestVisible = false;
            rect.Width = 124 * resolutionXMultiplier;
            rect.Height = chunkHeight;
            rect.Fill = buildingBlockAnimationFrames[timeCode][1];

            if (integerMultiplier == true)
            {
                //RenderOptions.SetBitmapScalingMode(imageBrush, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetBitmapScalingMode(rect, BitmapScalingMode.NearestNeighbor);
            }

            buildingBlocksRight.Add(rect);

            Canvas.SetLeft(rect, width - (BgBuildingsRectangle.Width + (45 * resolutionXMultiplier)));
            Canvas.SetTop(rect, chunkTop);

            FrontEndContainer.Children.Add(rect);
        }

        private void CrackRightSideOfTheBuilding(int chunkIndex)
        {
            double chunkTop = chunkHeight * chunkIndex;
            AddRightChunk(chunkTop);

            // crack two at a time if we're in portrait mode
            if ((chunksToCheck + 1) < numberOfBuildingChunks && portraitModeIndex == 1)
            {
                double secondChunkTop = chunkHeight * (chunkIndex + 1);
                AddRightChunk(secondChunkTop);
            }
        }
        
        private void CrackBuildingSound()
        {
            int rInt = r.Next(1, 3);
            
            if (rInt == 1)
            {
                BigBlue.XAudio2Player.PlaySound("buildingcrack1", null);
            }
            else
            {
                BigBlue.XAudio2Player.PlaySound("buildingcrack2", null);
            }
        }
        
        private bool CrackBuilding()
        {
            CrackBuildingSound();

            CrackRightSideOfTheBuilding(chunksToCheck);

            Rectangle item = buildingBlocks[chunksToCheck];
            
            int originalBlockCondition = (int)item.GetValue(FrameworkElement.TagProperty);

            if (originalBlockCondition == 0)
            {
                item.Fill = buildingBlockAnimationFrames[timeCode][1];
                item.SetValue(FrameworkElement.TagProperty, 1);
            }

            // crack two at a time if we're in portrait mode
            if ((chunksToCheck + 1) < numberOfBuildingChunks && portraitModeIndex == 1)
            {
                Rectangle secondItem = buildingBlocks[chunksToCheck + 1];

                int secondOriginalBlockCondition = (int)secondItem.GetValue(FrameworkElement.TagProperty);

                if (secondOriginalBlockCondition == 0)
                {
                    secondItem.Fill = buildingBlockAnimationFrames[timeCode][1];
                    secondItem.SetValue(FrameworkElement.TagProperty, 1);
                }
            }

            if (chunksToCheck == 0)
            {
                return true;
            }
            else
            {
                chunksToCheck = chunksToCheck - chunksPerTick;

                if (chunksToCheck < 0)
                {
                    chunksToCheck = 0;
                }

                return false;
            }
        }

        bool theEnd = false;

        private void FinalFight()
        {
            fightersElapsedMilliseconds = fightersElapsedMilliseconds + elapsedMilliseconds;

            //if (fightersTimer.ElapsedMilliseconds > 300)
            if (fightersElapsedMilliseconds > 300)
            {
                //fightersTimer.Reset();
                //fightersTimer.Start();
                fightersElapsedMilliseconds = 0;

                FightersRectangle.Fill = fightersAnimationFrames[timeCode][fighterFrameToDisplay];
                fighterCurrentFrame = fighterFrameToDisplay;
                //Fighters.Viewbox = fighterRects[fighterFrameToDisplay];

                switch (fighterFrameToDisplay)
                {
                    case 0:
                        theEnd = true;
                        break;
                    case 3:
                        fighterFrameToDisplay = 4;
                        break; 
                    case 4:
                        PunchFighter(0);
                        break;
                }
            }
        }

        long explosionSequenceDuration = 5000;

        private void AnimateExplosionSequence()
        {
            if (explosionTimer.ElapsedMilliseconds <= explosionSequenceDuration)
            {
                switch (explosionState)
                {
                    case ExplosionSequenceState.exploding:
                        // play explosion sound effect(s) and shake screen for the duration of the explosions sound
                        Shake();

                        if (georgeState == RampageState.climbing)
                        {
                            DestroyGeorge("suicide");
                        }

                        break;
                    case ExplosionSequenceState.settling:
                        // play sounds or something?
                        break;
                    case ExplosionSequenceState.cracking:
                        // play sounds?
                        break;
                    case ExplosionSequenceState.falling:
                        // play sounds?
                        break;
                    case ExplosionSequenceState.destroyed:
                        break;
                    case ExplosionSequenceState.punching:
                        

                        break;
                }
            }
            else
            {
                switch (explosionState)
                {
                    case ExplosionSequenceState.exploding:
                        // stabilize the screen and set the mode to settling
                        screenShaking = false;
                        FrontEndContainer.Margin = new Thickness(0, 0, 0, 0);
                        explosionSequenceDuration = 800;
                        explosionTimer.Stop();
                        explosionTimer.Reset();
                        explosionState = ExplosionSequenceState.settling;
                        explosionTimer.Start();
                        break;
                    case ExplosionSequenceState.settling:
                        // it'll take about 600ms for each crack
                        explosionSequenceDuration = 400;
                        explosionTimer.Stop();
                        explosionTimer.Reset();

                        

                        explosionState = ExplosionSequenceState.cracking;
                        explosionTimer.Start();
                        break;
                    case ExplosionSequenceState.cracking:                    
                        // crack sides of building and make george fall if he's alive
                        bool completelyCracked = CrackBuilding();

                        if (completelyCracked == true)
                        {
                            explosionTimer.Stop();
                            explosionTimer.Reset();
                            explosionTimer.Start();
                            explosionSequenceDuration = 280;
                            explosionState = ExplosionSequenceState.falling;
                        }
                        else
                        {
                            explosionTimer.Stop();
                            explosionTimer.Reset();
                            explosionTimer.Start();
                        }

                        break;
                    case ExplosionSequenceState.falling:
                        screenShaking = true;
                        BigBlue.XAudio2Player.PlayOverLappingSound("buildingstoreycrumble");
                        
                        bool completelyFlattened = FlattenBuilding();

                        if (completelyFlattened == true)
                        {
                            screenShaking = false;
                            explosionState = ExplosionSequenceState.destroyed;
                        }
                        else
                        {
                            // when building is lowered all the way stop shaking
                            // start shaking again and start to lower the building
                            explosionTimer.Stop();
                            explosionTimer.Reset();
                            explosionTimer.Start();
                        }
                        
                        break;
                    case ExplosionSequenceState.destroyed:
                        // after the building is kaput, stop the shaking again
                        screenShaking = false;
                        // stabilize the screen
                        FrontEndContainer.Margin = new Thickness(0, 0, 0, 0);
                        explosionSequenceDuration = 1000;
                        explosionTimer.Stop();
                        explosionTimer.Reset();
                        explosionTimer.Start();

                        explosionState = ExplosionSequenceState.punching;
                        
                        fighterFrameToDisplay = 3;
                        fighterCombo = true;
                        break;
                    case ExplosionSequenceState.punching:
                        // punch fighter in the face
                        // exit
                        // 3, 4, 2 animatioon frames

                        if (theEnd == true)
                        {
                            explosionSequenceDuration = 2200;
                            explosionTimer.Stop();
                            explosionTimer.Reset();
                            explosionTimer.Start();
                            explosionState = ExplosionSequenceState.ending;
                        }
                        else
                        {
                            FinalFight();
                        }
                        
                        break;
                    case ExplosionSequenceState.ending:
                        explosionState = ExplosionSequenceState.ended;
                        ExitFrontEnd(FrontEndContainer);
                        break;
                }

                //explosionTimer.Start();
            }
        }
        
        private void AnimateHaggar()
        {
            haggarElapsedMilliseconds = haggarElapsedMilliseconds + elapsedMilliseconds;

            //if (haggarTimer.ElapsedMilliseconds > (500 - haggerSpeed))
            if (haggarElapsedMilliseconds > (500 - haggerSpeed))
            {
                HaggarHeadRectangle.Fill = haggarAnimationFrames[haggarFrameToDisplay];
                //HaggarImage.Viewbox = haggarRects[haggarFrameToDisplay];

                //haggarTimer.Stop();
                //haggarTimer.Reset();
                //haggarTimer.Start();
                haggarElapsedMilliseconds = 0;

                if (haggerSpeed <= 390)
                {
                    haggerSpeed = haggerSpeed + 12;
                }

                if (haggarFrameToDisplay == 5)
                {
                    haggarFrameToDisplay = 0;
                }
                else
                {
                    haggarFrameToDisplay = haggarFrameToDisplay + 1;
                }
            }
        }
        
        bool shutdown = false;
        bool canThrowKnife = false;

        Storyboard shutdownStoryboard = new Storyboard();
        Storyboard regularlyScheduledProgrammingStoryboard = new Storyboard();
        Storyboard staticAfterExplosionStoryboard = new Storyboard();

        private bool FlattenBuilding()
        {
            foreach (Rectangle buildingBlock in buildingBlocks)
            {
                Canvas.SetTop(buildingBlock, Canvas.GetTop(buildingBlock) + (chunkHeight * chunksPerTick));
            }

            foreach (Rectangle buildingBlockDecal in buildingBlockDecals)
            {
                Canvas.SetTop(buildingBlockDecal, Canvas.GetTop(buildingBlockDecal) + (chunkHeight * chunksPerTick));
            }

            foreach (Rectangle buildingBLockRight in buildingBlocksRight)
            {
                Canvas.SetTop(buildingBLockRight, Canvas.GetTop(buildingBLockRight) + (chunkHeight * chunksPerTick));
            }

            Canvas.SetTop(ScreenDoor, Canvas.GetTop(ScreenDoor) + (chunkHeight * chunksPerTick));

            Canvas.SetBottom(BgBuildingsRectangle, Canvas.GetBottom(BgBuildingsRectangle) - (chunkHeight * chunksPerTick));
            Canvas.SetTop(BuildingRectangle, Canvas.GetTop(BuildingRectangle) + (chunkHeight * chunksPerTick));

            Canvas.SetTop(HaggarCanvas, Canvas.GetTop(HaggarCanvas) + (chunkHeight * chunksPerTick));
            Canvas.SetTop(ScreenBackgroundRectangle, Canvas.GetTop(ScreenBackgroundRectangle) + (chunkHeight * chunksPerTick));

            Canvas.SetTop(BuildingRoofRectangle, Canvas.GetTop(BuildingRoofRectangle) + (chunkHeight * chunksPerTick));

            if (Canvas.GetTop(BuildingRectangle) < height)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void RtypeExplosionComplete()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                rtypeShipState = RtypeState.dead;

                if (freeForAll == true)
                {
                    if (shutdown == false)
                    {
                        RespawnRtype(true);
                    }
                }
                else
                {
                    if (decidingWinner == false)
                    {
                        decidingWinner = true;
                        // start the decision timer
                        decisionTimer.Start();
                        //KeyTest.Text = "started rtype decision timer";
                    }
                }
            });
        }

        private void AnimateRtype()
        {
            rtypeElapsedMilliseconds = rtypeElapsedMilliseconds + elapsedMilliseconds;

            //if (rtypeDestroyed == true && (rtypeImageBrush.Viewbox == rtypeRects[0] || rtypeImageBrush.Viewbox == rtypeRects[1]))
            //if (rtypeDestroyed == true && (RtypeImage.Viewbox == rtypeRects[0] || RtypeImage.Viewbox == rtypeRects[1]))
            //if (Rtype.Fill == rtypeAnimationFrames[timeCode][1])
            if (rtypeDestroyed == true && (Rtype.Fill == rtypeAnimationFrames[timeCode][0] || Rtype.Fill == rtypeAnimationFrames[timeCode][1]))
            {
                //RtypeImage.Viewbox = rtypeRects[2];
                //rtypeImageBrush.Viewbox = rtypeRects[2];
                Rtype.Fill = rtypeAnimationFrames[timeCode][2];
                rtypeCurrentFrame = 2;
                PlasmaImage.Viewbox = new Rect(-300, -300, 0, 0);
                rtypeFrameToDisplay = 2;

                // playExplosionSound();
            }

            if (rtypeElapsedMilliseconds > 60)
            //if (rtypeTimer.ElapsedMilliseconds > 60)
            {
                if (rtypeFrameToDisplay == 11)
                {
                    if (RtypeFlyingStoryboard.GetCurrentState() != ClockState.Stopped)
                    {
                        RtypeFlyingStoryboard.Stop();
                    }
                }
                else
                {
                    //rtypeImageBrush.Viewbox = rtypeRects[rtypeFrameToDisplay];
                    Rtype.Fill = rtypeAnimationFrames[timeCode][rtypeFrameToDisplay];
                    rtypeCurrentFrame = rtypeFrameToDisplay;
                    //RtypeImage.Viewbox = rtypeRects[rtypeFrameToDisplay];
                }

                rtypeElapsedMilliseconds = 0;
                //rtypeTimer.Stop();
                //rtypeTimer.Reset();
                //rtypeTimer.Start();

                if (rtypeDestroyed == true && rtypeFrameToDisplay > 1 && rtypeFrameToDisplay < 11)
                {
                    rtypeFrameToDisplay = rtypeFrameToDisplay + 1;
                }
            }
        }
        
        private void AnimateSpectators()
        {
            crowdElapsedMilliseconds = crowdElapsedMilliseconds + elapsedMilliseconds;

            //if (crowdTimer.ElapsedMilliseconds > 250)
            if (crowdElapsedMilliseconds > 250)
            {
                //crowdTimer.Reset();
                //crowdTimer.Start();
                crowdElapsedMilliseconds = 0;

                foreach (Rectangle spectatorTile in spectatorTiles)
                {
                    spectatorTile.Fill = spectatorAnimations[streetFighterEditionAnimation][timeCode][spectatorsFrameToDisplay];
                }

                spectatorsCurrentFrame = spectatorsFrameToDisplay;

                if (spectatorForward == true)
                {
                    spectatorsFrameToDisplay = spectatorsFrameToDisplay + 1;
                }
                else
                {
                    spectatorsFrameToDisplay = spectatorsFrameToDisplay - 1;
                }

                if (spectatorsFrameToDisplay == 0)
                {
                    spectatorForward = true;
                }

                if (spectatorsFrameToDisplay == 2)
                {
                    spectatorForward = false;
                }
            }
        }

        private void FightAnnouncementCleanup()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                FightRectangle.Visibility = System.Windows.Visibility.Hidden;
                
                GeorgeClimbingStoryBoard.Begin();
                RtypeFlyingStoryboard.Begin();

                challengerIntroRunning = false;
                roundInProgress = true;
                ResumeFrontend();
                roundTimer.Start();
            });
        }

        private void PlayRampageSlipping()
        {
            BigBlue.XAudio2Player.PlaySound("wtf", null);
        }

        private void PlayGeorgeScream()
        {
            BigBlue.XAudio2Player.PlaySound("georgescream", null);
        }

        private void PlayGeorgeCrash()
        {
            BigBlue.XAudio2Player.PlaySound("crash", null);
        }

        public void VictoryCleanup()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                decidingWinner = false;
                decisionMade = true;
                celebratingVictory = false;

                //WinnerRectangle.Opacity = 0;
                WinnerRectangle.Visibility = System.Windows.Visibility.Hidden;

                ResumeFrontend();
                
                RespawnUi();

                // we put this afterwards so we can check on it for the shutdown sequence
                showWinnerText = false;
            });
        }

        public void NewChallengerCleanup()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                newChallengerTimer.Stop();
                challengerJoined = false;
                ShowVersusScreen();
            });
        }
        
        private async void PlayVictoryDitty()
        {
            if (champion == 0)
            {
                RampageMonsterRectangle.Fill = rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][11];
                rampageCurrentFrame = 11;

                BigBlue.XAudio2Player.PlaySound("georgevictory", null);
            }
            else
            {
                //rtypeReturnToNormalSpeed();
                BigBlue.XAudio2Player.PlaySound("rtypevictory", null);
            }

            if (BigBlue.XAudio2Player.Disabled)
            {
                awaitingAsync = true;

                await Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(3000);
                });

                awaitingAsync = false;

                VictoryCleanup();
            }
        }

        private void PlayExplosionSound()
        {
            BigBlue.XAudio2Player.StopSound("warp");

            BigBlue.XAudio2Player.PlaySound("explosion", GetRtypeSpeakerPosition(currentRtypeLeftPosition + (rtypeWidth / 2)));
        }

        private void PlayRtypeLaserSound()
        {
            BigBlue.XAudio2Player.PlaySound("laser", GetRtypeSpeakerPosition(currentRtypeLeftPosition + rtypeWidth));
        }

        private void LaunchGame()
        {
            // set george to the spectating (healthy) frame
            if (georgeState != RampageState.falling && georgeState != RampageState.burning)
            {
                //RampageImage.Viewbox = georgeRects[7];
                RampageMonsterRectangle.Fill = rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][7];
                rampageCurrentFrame = 7;
            }

            // pause the frontend
            PauseFrontend();
            
            // play the fighter punch sound effect
            PlayFighterPunchSound();

            // fade out the screen to black;
            LaunchGameStoryboard.Begin();

            // change the frontend so that it's not the topmost focused window
            //FrontEndWindow.Topmost = false;
        }

        private void PunchFighter(int nextFighterFrame)
        {
            // if george isn't falling, dead, or in the middle of a punch, we're going to change him to the spectating frame
            //if (georgeState != rampageState.dead && georgeState != rampageState.falling && georgeState != rampageState.burning && RampageImage.Viewbox != georgeRects[4] && RampageImage.Viewbox != georgeRects[5] && RampageImage.Viewbox != georgeRects[6])
            if (georgeState != RampageState.dead && georgeState != RampageState.falling && georgeState != RampageState.burning && RampageMonsterRectangle.Fill != rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][4] && RampageMonsterRectangle.Fill != rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][5] && RampageMonsterRectangle.Fill != rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][6])        
            {
                georgeState = RampageState.spectating;
            }

            // special punch for launching a game
            if (gameLaunchSoundFinished == true)
            {
                gameLaunchSoundFinished = false;
                LaunchGame();
            }
            else
            {
                // play the punch sound effect
                PlayFighterPunchSound();

                if (Blood.Visibility == System.Windows.Visibility.Hidden)
                {
                    Blood.Visibility = System.Windows.Visibility.Visible;
                    Blood2.Visibility = System.Windows.Visibility.Visible;
                    Blood3.Visibility = System.Windows.Visibility.Visible;
                    Blood4.Visibility = System.Windows.Visibility.Visible;
                }

                // animate zulu blood
                blood1Storyboard.Begin(this, true);
                blood2Storyboard.Begin(this, true);
                blood3Storyboard.Begin(this, true);
                blood4Storyboard.Begin(this, true);

                fighterFrameToDisplay = nextFighterFrame;
            }
        }
        
        private void AnimateFighters()
        {
            //fightersElapsedMilliseconds = fightersElapsedMilliseconds + elapsedMilliseconds;

            if (fightersElapsedMilliseconds > 300)
            //if (fightersTimer.ElapsedMilliseconds > 300)
            {
                fightersElapsedMilliseconds = 0;
                //fightersTimer.Reset();
                //fightersTimer.Start();

                FightersRectangle.Fill = fightersAnimationFrames[timeCode][fighterFrameToDisplay];
                fighterCurrentFrame = fighterFrameToDisplay;
                //Fighters.Viewbox = fighterRects[fighterFrameToDisplay];

                if (fighterFrameToDisplay < 3)
                {
                    fighterCombo = false;
                }

                switch (fighterFrameToDisplay)
                {
                    case 0:
                        if (georgeState != RampageState.dead && georgeState != RampageState.falling && georgeState != RampageState.burning)
                        {
                            georgeState = RampageState.climbing;
                        }

                        fighterFrameToDisplay = 1;
                        break;
                    case 1:
                        fighterFrameToDisplay = 2;
                        break;
                    case 2:
                        fighterFrameToDisplay = 0;
                        break;
                    case 3:
                        fighterFrameToDisplay = 4;
                        break;
                    case 4:
                        PunchFighter(5);
                        break;
                    case 5:
                        if (fighterCombo == false)
                        {
                            fighterFrameToDisplay = 0;
                        }
                        else
                        {
                            fighterFrameToDisplay = 4;
                        }
                        break;
                }
            }
        }

        private void PlayFighterPunchSound()
        {
            BigBlue.XAudio2Player.PlaySound("punch", null);
        }
        
        private void PlayGeorgePunchSound()
        {
            BigBlue.XAudio2Player.PlaySound("lightpunch", null);
        }

        private void PlayTruckedBuildingSound()
        {
            BigBlue.XAudio2Player.PlaySound("breakwindow", null);
        }

        private void CalculateLifeBarOpacity(double georgeCurrentPosition)
        {
            if (roundInProgress == true)
            {
                double lifeBarOpacity = KOCountdown.Opacity;

                if (surroundPosition == BigBlue.Models.SurroundPosition.None)
                {
                    if (georgeCurrentPosition <= (30 * resolutionXMultiplier) && lifeBarOpacity == 1)
                    {
                        KOCountdown.Opacity = 0.4;
                    }

                    if (georgeCurrentPosition > (30 * resolutionXMultiplier) && lifeBarOpacity == 0.4)
                    {
                        KOCountdown.Opacity = 1;
                    }
                    
                }

                if (surroundPosition == BigBlue.Models.SurroundPosition.Up || surroundPosition == BigBlue.Models.SurroundPosition.Down)
                {
                    
                        if (georgeCurrentPosition <= surroundGameListWidthOffset + (30 * resolutionXMultiplier) && georgeCurrentPosition >= surroundGameListWidthOffset && lifeBarOpacity == 1)
                        {
                            KOCountdown.Opacity = 0.4;
                            return;
                        }

                        if (georgeCurrentPosition >= (surroundGameListWidthOffset + (30 * resolutionXMultiplier)) && lifeBarOpacity == 0.4)
                        {
                            KOCountdown.Opacity = 1;
                            return;
                        }
                    
                    
                }
            }
        }

        private void GeorgeClimbing()
        {
            CalculateLifeBarOpacity(currentRampageTopPosition);

            if (currentRampageTopPosition < climbingDownPoint)
            {
                climbingUp = false;
            }

            if (currentRampageTopPosition >= climbingUpPoint)
            {
                climbingUp = true;
            }
        }

        private void GeorgeFalling()
        {
            CalculateLifeBarOpacity(currentRampageTopPosition);

            if (currentRampageTopPosition + (Math.Ceiling(285 * resolutionXMultiplier)) <= height)
            {
                Canvas.SetTop(GeorgeDamageDecal, currentRampageTopPosition + decalOffset);
            }
            else
            {
                Canvas.SetTop(GeorgeDamageDecal, height);
                georgeState = RampageState.dead;
                screenShaking = true;
                PlayGeorgeCrash();
                screenShakeTimer.Start();
                rampageFrameToDisplay = 0;
            }
        }

        double oneThirdScreenWidth;
        double twoThirdsScreenWidth;

        private string GetRtypeSpeakerPosition(double position)
        {
            string newSpeakerPosition = string.Empty;
            
            if (position >= -rtypeWidth && position <= oneThirdScreenWidth)
            {
                newSpeakerPosition = "left";
            }
            else if (position > oneThirdScreenWidth && position <= twoThirdsScreenWidth)
            {
                newSpeakerPosition = "center";
            }
            else if (position > twoThirdsScreenWidth)
            {
                newSpeakerPosition = "right";
            }
            else
            {
                newSpeakerPosition = warpSpeakerPosition;
            }

            return newSpeakerPosition;
        }

        private void RtypeFlying()
        {
            if (rtypeShipState == RtypeState.warping)
            {    
                string newSpeakerPosition = GetRtypeSpeakerPosition(currentRtypeLeftPosition);

                if (warpSpeakerPosition != newSpeakerPosition)
                {
                    warpSpeakerPosition = newSpeakerPosition;
                    BigBlue.XAudio2Player.AdjustSpeakerAssignment("warp", warpSpeakerPosition);
                }
                
                if (warpStopWatch.IsRunning == false)
                {
                    warpStopWatch.Start();

                    if (georgeState != RampageState.falling && georgeState != RampageState.dead && georgeState != RampageState.burning)
                    {
                        double newRtypeLifeBarWidth = Two.Width - (50 * resolutionXMultiplier);

                        if (newRtypeLifeBarWidth > 0)
                        {
                            Two.Width = newRtypeLifeBarWidth;
                        }
                        else
                        {
                            Two.Width = 0;
                        }
                    }
                }
            }
            
            /*
            if (currentRtypePosition <= width && currentRtypePosition >= 0)
            {
                panningPosition = (2 * (-(float)currentRtypePosition / (float)width)) + 1;
            }
            else
            {
                panningPosition = -1.0f;
                //currentRtypePosition = -(236 * resolutionXMultiplier);
            }
            */
                
            //double currentCollisionPosition = Canvas.GetLeft(RtypeCollisionBox);

            //if (currentRtypeLeftPosition != currentCollisionPosition)
            //{
              //  Canvas.SetLeft(RtypeCollisionBox, currentRtypeLeftPosition);
                Canvas.SetLeft(RtypePlasma, currentRtypeLeftPosition - rtypeShipOnlyLength);
            //}
        }

        private void CalculateRtypeResource()
        {
            if (georgeState != RampageState.falling && georgeState != RampageState.dead && georgeState != RampageState.burning)
            {
                //KeyTest.Text = "george is not falling and george is not dead";
                // check to see whether rtype is out of fuel
                if (Two.Width <= 0)
                {
                    DestroyRtype();
                    warpStopWatch.Reset();
                    return;
                }

                if (warpStopWatch.ElapsedMilliseconds >= 1000)
                {
                    double newRtypeLifeBarWidth = Two.Width - (25 * resolutionXMultiplier);

                    if (newRtypeLifeBarWidth > 0)
                    {
                        Two.Width = newRtypeLifeBarWidth;
                    }
                    else
                    {
                        Two.Width = 0;
                    }

                    warpStopWatch.Reset();
                    warpStopWatch.Start();
                }
            }            
        }

        double currentRtypeLeftPosition;

        private void ManageRtype()
        {
            if (rtypeShipState != RtypeState.dead)
            {
                currentRtypeLeftPosition = Canvas.GetLeft(Rtype);

                if (rtypeLaserFired == true)
                {
                    rtypeLaserFired = false;
                    RtypeShootLaser();
                    rtypeElapsedMilliseconds = 0;
                    //rtypeTimer.Reset();
                    //rtypeTimer.Start();
                }

                if (rtypeDestroyed == false)
                {
                    if (roundInProgress == true)
                    {
                        CalculateRtypeResource();
                    }
                
                    RtypeFlying();
                    AnimatePlasma();
                }

                AnimateRtype();
            }
        }

        private void PlayGeorgeSwingSound()
        {
            BigBlue.XAudio2Player.PlaySound("swing", null);
        }

        private void GeorgeAnimatePunches()
        {
            //if ((georgeLeftPunch == true || georgeRightPunch == true) && RampageImage.Viewbox != georgeRects[4] && RampageImage.Viewbox != georgeRects[5] && RampageImage.Viewbox != georgeRects[6])
            if ((georgeLeftPunch == true || georgeRightPunch == true) && RampageMonsterRectangle.Fill != rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][4] && RampageMonsterRectangle.Fill != rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][5] && RampageMonsterRectangle.Fill != rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][6])        
            {
                if (roundInProgress == false)
                {
                    GeorgeClimbingStoryBoard.Pause();
                }

                if (georgeLeftPunch == true)
                {
                    PlayGeorgeSwingSound();

                    georgeLeftPunch = false;
                    //RampageImage.Viewbox = georgeRects[4];
                    RampageMonsterRectangle.Fill = rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][4];
                    rampageCurrentFrame = 4;
                }

                if (georgeRightPunch == true)
                {
                    georgeRightPunch = false;
                    GeorgePunchBuilding();

                    if (climbingUp == false)
                    {
                        //RampageImage.Viewbox = georgeRects[6];
                        RampageMonsterRectangle.Fill = rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][6];
                        rampageCurrentFrame = 6;
                    }
                    else
                    {
                        //RampageImage.Viewbox = georgeRects[5];
                        RampageMonsterRectangle.Fill = rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][5];
                        rampageCurrentFrame = 5;
                    }
                }

                if (climbingUp == false)
                {
                    rampageFrameToDisplay = 2;
                }
                else
                {
                    rampageFrameToDisplay = 0;
                }

                rampageElapsedMilliseconds = 0;
                //georgeTimer.Reset();
                //georgeTimer.Start();
            }
        }

        private void CalculateGeorgeResource()
        {
            if (rtypeDestroyed == false && rtypeShipState != RtypeState.dead)
            {
                // check to see whether geroge's grip is loose
                if (One.Width <= 0)
                {
                    DestroyGeorge("suicide");
                    spectateStopWatch.Reset();
                    return;
                }

                if (spectateStopWatch.ElapsedMilliseconds >= 1000)
                {
                    double newGeorgeLifeBarWidth = One.Width - (25 * resolutionXMultiplier);

                    if (newGeorgeLifeBarWidth > 0)
                    {
                        One.Width = newGeorgeLifeBarWidth;
                    }
                    else
                    {
                        One.Width = 0;
                    }

                    spectateStopWatch.Reset();
                    spectateStopWatch.Start();
                }
            }            
        }
        
        private void ManageGeorge(long elapsedMilliseconds)
        {
            if (georgeState != RampageState.dead)
            {
                currentRampageTopPosition = Canvas.GetTop(RampageMonsterRectangle);

                georgeBodyCollisionRect.Y = currentRampageTopPosition + georgeBodyCollisionOffset;
                georgeCollisionRect1.Y = currentRampageTopPosition + georgeCollision1Offset;
                georgeCollisionRect2.Y = currentRampageTopPosition + georgeCollision2Offset;
                georgeCollisionRect3.Y = currentRampageTopPosition + georgeCollision3Offset;
                
                if (georgeState != RampageState.falling && georgeState != RampageState.burning && roundInProgress == true)
                {
                    CalculateGeorgeResource();
                }
            }

            switch (georgeState)
            {
                case RampageState.climbing:
                    if (spectateStopWatch.IsRunning == true)
                    {
                        spectateStopWatch.Reset();
                    }

                    GeorgeAnimatePunches();

                    GeorgeClimbing();

                    //if (GeorgeClimbingStoryBoard.GetIsPaused() == true)
                    //{
                    //GeorgeClimbingStoryBoard.Resume();
                    //}
                    
                    break;
                case RampageState.spectating:
                    if (spectateStopWatch.IsRunning == false && roundInProgress == true && rtypeDestroyed == false && rtypeShipState != RtypeState.dead)
                    {
                        spectateStopWatch.Start();

                        double newGeorgeLifebarWidth = One.Width - (50 * resolutionXMultiplier);

                        if (newGeorgeLifebarWidth > 0)
                        {
                            One.Width = newGeorgeLifebarWidth;
                        }
                        else
                        {
                            One.Width = 0;
                        }
                    }

                    if (GeorgeClimbingStoryBoard.GetIsPaused() == false)
                    {
                        GeorgeClimbingStoryBoard.Pause();
                    }

                    // do a health check here to decide which spectating frame to show; we never want to show the pain spectating frame when a match isn't in progress
                    if (roundInProgress == true && One.Width <= lowHealthWidth)
                    {
                        //RampageImage.Viewbox = georgeRects[8];
                        RampageMonsterRectangle.Fill = rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][8];
                        rampageCurrentFrame = 8;
                    }
                    else
                    {
                        //RampageImage.Viewbox = georgeRects[7];
                        RampageMonsterRectangle.Fill = rampageMonsterAnimationFrames[rampageCharacterChoice][timeCode][7];
                        rampageCurrentFrame = 7;
                    }

                    return;
                    
                case RampageState.falling:
                    GeorgeFalling();
                    
                    break;
            }

            AnimateGeorge();
        }

        private void ManageFighters()
        {
            fightersElapsedMilliseconds = fightersElapsedMilliseconds + elapsedMilliseconds;

            //ImageBrush fightersBrush = (ImageBrush)FightersRectangle.Fill;
            //if (fighterAttacking == true && FightersRectangle.Fill != fightersAnimationFrames[timeCode][3] && FightersRectangle.Fill != fightersAnimationFrames[timeCode][4])
            if (fighterAttacking == true && fighterCurrentFrame != 3 && fighterCurrentFrame != 4)
            {
                //if (fightersTimer.ElapsedMilliseconds < 300 && fighterCurrentFrame == 0)
                if (fightersElapsedMilliseconds < 300 && fighterCurrentFrame == 0)
                {
                    fighterFrameToDisplay = 3;
                }
                else
                {
                    FightersRectangle.Fill = fightersAnimationFrames[timeCode][3];
                    fighterCurrentFrame = 3;
                    //Fighters.Viewbox = fighterRects[3];
                    fighterFrameToDisplay = 4;
                    fightersElapsedMilliseconds = 0;
                    //fightersTimer.Reset();
                    //fightersTimer.Start();
                }
            }
            
            AnimateFighters();
        }

        private void ManageBuilding()
        {
            AnimateBuildingChunkDecal();
            AnimateDebris();
        }
                
        private void AnimateLifeBars()
        {
            userInterfaceElapsedMilliseconds = userInterfaceElapsedMilliseconds + elapsedMilliseconds;

            //if (uiTimer.ElapsedMilliseconds > 60)
            if (userInterfaceElapsedMilliseconds > 60)
            {
                if (One.Width <= lowHealthWidth || Two.Width <= lowHealthWidth)
                {
                    // since the players can die in one hit, we don't want to switch to the fast music if they're dead
                    if (fastMusic == false && music == true && shutdown == false && One.Width > 0 && Two.Width > 0)
                    {
                        fastMusic = true;
                        BigBlue.XAudio2Player.StopSound("guile");
                        BigBlue.XAudio2Player.PlaySound("guilefast", null);
                    }

                    if (rtypeDestroyed == true || georgeState == RampageState.falling || georgeState == RampageState.dead || georgeState == RampageState.burning)
                    {
                        KOImage.Viewbox = koRects[0];
                        knockOutFrameToDisplay = 0;
                    }
                    else
                    {
                        KOImage.Viewbox = koRects[knockOutFrameToDisplay];

                        if (knockOutFrameToDisplay == 0)
                        {
                            knockOutFrameToDisplay = 1;
                        }
                        else
                        {
                            knockOutFrameToDisplay = 0;
                        }
                    }
                }

                if (roundDuration < 15)
                {
                    FirstDigit.Fill = numbers[currentDigitOne + numberType];
                    SecondDigit.Fill = numbers[currentDigitTwo + numberType];

                    if (numberType == "0")
                    {
                        numberType = "1";
                    }
                    else
                    {
                        numberType = "0";
                    }
                }

                //uiTimer.Stop();
                //uiTimer.Reset();
                //uiTimer.Start();
                userInterfaceElapsedMilliseconds = 0;
            }
        }

        

        int winnerTextFrameToDisplay = 0;

        private void AnimateWinnerText()
        {
            if (winnerTextTimer.ElapsedMilliseconds <= 50)
            {
                return;
            }
            else
            {
                winnerTextTimer.Stop();
                winnerTextTimer.Reset();
                winnerTextTimer.Start();

                WinnerImage.Viewbox = winnerTextRects[winnerTextFrameToDisplay];

                if (champion == 0)
                {
                    switch (rampageCharacterChoice)
                    {
                        case 0:
                            if (winnerTextFrameToDisplay == 0)
                            {
                                winnerTextFrameToDisplay = 1;
                            }
                            else
                            {
                                winnerTextFrameToDisplay = 0;
                            }
                            break;
                        case 1:
                            if (winnerTextFrameToDisplay == 2)
                            {
                                winnerTextFrameToDisplay = 3;
                            }
                            else
                            {
                                winnerTextFrameToDisplay = 2;
                            }
                            break;
                        case 2:
                            if (winnerTextFrameToDisplay == 4)
                            {
                                winnerTextFrameToDisplay = 5;
                            }
                            else
                            {
                                winnerTextFrameToDisplay = 4;
                            }
                            break;
                    }    
                }

                if (champion == 1)
                {
                    if (winnerTextFrameToDisplay == 6)
                    {
                        winnerTextFrameToDisplay = 7;
                    }
                    else
                    {
                        winnerTextFrameToDisplay = 6;
                    }
                }
            }
        }

        private void ManageUI()
        {
            userInterfaceElapsedMilliseconds = userInterfaceElapsedMilliseconds + elapsedMilliseconds;

            //if (freeForAll == false && itsGoTime == false && shutdownSequenceActivated == false && decisionMade == false && uiTimer.ElapsedMilliseconds > 400)
            if (freeForAll == false && itsGoTime == false && shutdownSequenceActivated == false && decisionMade == false && userInterfaceElapsedMilliseconds > 400)
            {
                if (PressStartRectangle.Visibility == System.Windows.Visibility.Visible)
                {
                    PressStartRectangle.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {   
                    PressStartRectangle.Visibility = System.Windows.Visibility.Visible;
                }

                userInterfaceElapsedMilliseconds = 0;
                //uiTimer.Stop();
                //uiTimer.Reset();
                //uiTimer.Start();
            }
        }

        long elapsedMilliseconds = 0;
        
        public override void OnFrame(object sender, EventArgs e)
        {    
            if (menuOpen == false)
            {
                elapsedMilliseconds = genericAnimationStopWatch.ElapsedMilliseconds - elapsedMilliseconds;

                //KeyTest.Text = elapsedMilliseconds.ToString();

                ManageVideo();

                if (challengerIntroRunning == false && challengerJoined == false)
                {
                    if (pausedBySystem == false)
                    {
                        ManageGeorge(elapsedMilliseconds);
                        ManageRtype();
                        ManageBuilding();

                        ShakeScreen();

                        if (shutdownSequenceActivated == true)
                        {
                            if (haggarInDanger == true)
                            {
                                AnimateFuse();
                                AnimateHaggar();
                                AnimateIgnite();
                            }
                            else
                            {
                                AnimateStatic();
                            }
                        }
                    }
                        
                    if (shutdown == false)
                    {
                        CalculateLaserCollisions();

                        if (freeForAll == true || (freeForAll == false && roundInProgress == true))
                        {
                            CalculatePlasmaCollisions();

                            CalculateRtypeCollision();

                            if (roundInProgress == true && pausedBySystem == false)
                            {
                                if (One.Width > 0 && Two.Width > 0)
                                {
                                    if (shutdownSequenceActivated == false)
                                    {
                                        RoundCountdown();
                                    }

                                    if (One.Width <= lowHealthWidth || Two.Width <= lowHealthWidth || roundDuration < 15)
                                    {
                                        AnimateLifeBars();
                                    }
                                }
                            }
                        }

                        if (roundInProgress == false)
                        {
                            if (pausedBySystem == false)
                            {
                                ManageGameList();

                                ManageUI();
                            }
                        }
                        else
                        {
                            if (decidingWinner == true)
                            {
                                MakeFightDecision();
                            }

                            if (waitingForDeathAnimations == true)
                            {
                                WaitForDeathAnimations();
                            }

                            if (celebratingVictory == true)
                            {
                                RenderDecision();

                                if (showWinnerText == true)
                                {
                                    AnimateWinnerText();
                                }
                            }
                        }

                        if (pausedBySystem == false)
                        {
                            ManageFighters();
                            AnimateSpectators();
                        }
                    }
                    else
                    {
                        //canThrowKnife = false;
                        AnimateExplosionSequence();
                    }

                    // old spot
                }
                else
                {
                    if (challengerJoined == true)
                    {
                        AnimateNewChallengerText();
                    }
                }

                elapsedMilliseconds = genericAnimationStopWatch.ElapsedMilliseconds;
            }
        }

        private void AnimatePlasma()
        {
            plasmaElapsedMilliseconds = plasmaElapsedMilliseconds + elapsedMilliseconds;

            if (rtypeShipState == RtypeState.warping)
            {
                if (RtypePlasma.Visibility == System.Windows.Visibility.Hidden)
                {
                    RtypePlasma.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                if (RtypePlasma.Visibility == System.Windows.Visibility.Visible)
                {
                    RtypePlasma.Visibility = System.Windows.Visibility.Hidden;
                }

                // don't care about animating
                return;
            }
            
            //if (plasmaTimer.ElapsedMilliseconds > 150)
            if (plasmaElapsedMilliseconds > 150)
            {
                plasmaElapsedMilliseconds = 0;
                //plasmaTimer.Reset();
                //plasmaTimer.Start();

                if (PlasmaImage.Viewbox == plasmaRects[0])
                {
                    PlasmaImage.Viewbox = plasmaRects[1];
                }
                else if (PlasmaImage.Viewbox == plasmaRects[1])
                {
                    PlasmaImage.Viewbox = plasmaRects[2];
                }
                else if (PlasmaImage.Viewbox == plasmaRects[2])
                {
                    PlasmaImage.Viewbox = plasmaRects[0];
                }
            }
        }
        
        private void CreateVideoControl()
        {
            MediaElement me = new MediaElement();
            me.Opacity = 0;
            me.Width = snapShotWidth;
            me.Height = snapShotHeight;
            me.IsHitTestVisible = false;
            me.Stretch = Stretch.Fill;
            me.MediaFailed += VideoElement_MediaFailed;
            me.LoadedBehavior = MediaState.Manual;
            //me.MediaEnded += gameVideo_MediaEnded;

            VideoElement = me;
        }

        private void ResetVideoControl()
        {
            StopVideo();
            videoMe = null;
            //Video.Children.Remove(VideoElement);

            CreateVideoControl();
            videoFadeInStopwatch.Start();
        }

        private void VideoElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            //MessageBox.Show(e.ErrorException.Message);
            ResetVideoControl();
        }
    }
}