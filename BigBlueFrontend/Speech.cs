using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BigBlue
{
    public static class Speech
    {
        internal static bool IsNoiseWord(string wLower)
        {
            if (string.Equals(wLower, "the", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "a", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "of", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "at", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "as", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "and", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "ii", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "to", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "n'", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "'n", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "a", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "b", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "x", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "in", StringComparison.InvariantCultureIgnoreCase) || string.Equals(wLower, "on", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        internal static void InitializeVoiceRecognition(BigBlueWindow bbWindow)
        {
            try
            {
                bbWindow.recognizer = new SpeechRecognitionEngine();
                bbWindow.recognizer.InitialSilenceTimeout = TimeSpan.FromSeconds(4.0);
                bbWindow.recognizer.RecognizeCompleted +=
                    new EventHandler<RecognizeCompletedEventArgs>((sender, e) => SpeechRecognizeCompleted(sender, e, bbWindow));

                Choices listItemChoices = new Choices();
                listItemChoices.Add(new Choices(bbWindow.voiceChoices.ToArray()));

                GrammarBuilder chooseListItems = new GrammarBuilder();
                chooseListItems.Append(listItemChoices);

                Grammar searchListItemsGrammar = new Grammar(chooseListItems);
                searchListItemsGrammar.Name = "Search List Items";

                bbWindow.recognizer.LoadGrammarAsync(searchListItemsGrammar);
                bbWindow.recognizer.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>((sender, e) => BigBlue.Speech.SpeechHypothesized(sender, e, bbWindow));
                bbWindow.recognizer.SetInputToDefaultAudioDevice();
                bbWindow.recognizer.RecognizeAsyncStop();
            }
            catch (Exception)
            {
                bbWindow.recognizer = null;
            }
        }

        internal static void SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e, BigBlueWindow bbWindow)
        {
            if (!IsNoiseWord(e.Result.Text))
            {
                if (!bbWindow.MatchingSearchWords.Contains(e.Result.Text, StringComparer.InvariantCultureIgnoreCase))
                {
                    // need some standards; require a score of at least 0.5
                    if (e.Result.Confidence >= 0.5)
                    {
                        // if we're very confident, fuck the rest
                        if (e.Result.Confidence >= 0.9)
                        {
                            bbWindow.MatchingSearchWords.Clear();
                        }

                        bbWindow.MatchingSearchWords.Add(e.Result.Text);
                    }
                }
            }
        }

        // Handle the RecognizeCompleted event.
        internal static void SpeechRecognizeCompleted(object sender, RecognizeCompletedEventArgs e, BigBlueWindow bbWindow)
        {
            if (e.Error != null)
            {
                bbWindow.recognizer.RecognizeAsyncCancel();
            }

            if (e.InitialSilenceTimeout || e.BabbleTimeout)
            {
                //recognizer.RecognizeAsyncStop();
                bbWindow.recognizer.RecognizeAsyncCancel();
            }

            if (bbWindow.MatchingSearchWords.Count() > 0)
            {
                bool searchMatch = false;

                foreach (Models.FrontendListItem l in bbWindow.frontendLists.Values)
                {
                    // you have to exclude the actual search list otherwise you're just going to keep getting more and more duplicates
                    if (l.ListItems != null && l.ID != bbWindow.searchListGuid)
                    {
                        foreach (Models.FrontendListItem li in l.ListItems)
                        {
                            foreach (string searchCommand in bbWindow.MatchingSearchWords)
                            {
                                // instead of this duplicate crap, it needs to actually do a comparison on multiple values
                                // it needs to check if the title and exe are the same
                                // if they are, screw that crap

                                if (!string.IsNullOrWhiteSpace(li.SearchTitle))
                                {
                                    if (li.SearchTitle.IndexOf(searchCommand, StringComparison.InvariantCultureIgnoreCase) >= 0 && string.IsNullOrWhiteSpace(li.ParentFolder))
                                    {
                                        if (!searchMatch)
                                        {
                                            bbWindow.frontendLists[bbWindow.searchListGuid].ListItems.Clear();
                                            searchMatch = true;
                                        }

                                        IEnumerable<Models.FrontendListItem> duplicates = bbWindow.frontendLists[bbWindow.searchListGuid].ListItems.Where(n => n.SearchTitle == li.SearchTitle && n.Binary == li.Binary);

                                        if (duplicates.Count() == 0)
                                        {
                                            bbWindow.frontendLists[bbWindow.searchListGuid].ListItems.Add(li);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                int itemCount = bbWindow.frontendLists[bbWindow.searchListGuid].ListItems.Count();

                if (itemCount > 0)
                {
                    bbWindow.frontendLists[bbWindow.selectedListGuid].CurrentListIndex = bbWindow.selectedListItemIndex;
                    bbWindow.frontendLists[bbWindow.searchListGuid].ListItems = new ObservableCollection<Models.FrontendListItem>(bbWindow.frontendLists[bbWindow.searchListGuid].ListItems.Where(x => x.Title != "").OrderByDescending(n => n.ChildID.HasValue).ThenBy(o => o.Title));
                    bbWindow.selectedListItemIndex = 0;
                    bbWindow.selectedListGuid = bbWindow.searchListGuid;
                    bbWindow.frontendLists[bbWindow.searchListGuid].CurrentListIndex = 0;

                    if (bbWindow.textBlockListItemsToPage > itemCount)
                    {
                        bbWindow.frontendLists[bbWindow.searchListGuid].TextBlockItemsToPage = 1;
                    }
                    else
                    {
                        bbWindow.frontendLists[bbWindow.searchListGuid].TextBlockItemsToPage = bbWindow.textBlockListItemsToPage;
                    }

                    if (bbWindow.imageBlockListItemsToPage > itemCount)
                    {
                        bbWindow.frontendLists[bbWindow.searchListGuid].ImageItemsToPage = 1;
                    }
                    else
                    {
                        bbWindow.frontendLists[bbWindow.searchListGuid].ImageItemsToPage = bbWindow.imageBlockListItemsToPage;
                    }

                    bbWindow.frontendLists[bbWindow.searchListGuid].Total = itemCount - 1;

                    bbWindow.Dispatcher.Invoke((Action)delegate
                    {
                        bbWindow.SelectList(bbWindow.searchListGuid, false);
                        bbWindow.textBlockListCanvas.Visibility = Visibility.Visible;
                        bbWindow.awaitingAsync = false;
                    });

                    XAudio2Player.PlaySound(bbWindow.SelectListSoundKey, null);
                    return;
                }
            }

            bbWindow.Dispatcher.Invoke((Action)delegate
            {
                bbWindow.textBlockListCanvas.Visibility = Visibility.Visible;
                bbWindow.SetGameSnapshots(false);
                bbWindow.ResetVideo();
            });

            bbWindow.PlayLoseSound();
        }
    }
}
