import * as SpeechSDK from "microsoft-cognitiveservices-speech-sdk";
const key = "the cognitiveservices key - but in real app you should get a token from the backend to not expose the key";
const region = "westeurope";

let player: SpeechSDK.SpeakerAudioDestination;

export function stopSynthesis() {
  player.pause();
}

export function startSynthesis(
  text: string,
  language: string,
  startCallback: any,
  stopCallback: any
) {
  const speechConfig = SpeechSDK.SpeechConfig.fromSubscription(key, region);
  speechConfig.speechSynthesisVoiceName = "en-US-EmmaNeural"; //"en-US-BrianNeural"; //"en-US-JennyNeural";
  speechConfig.speechSynthesisLanguage = language;
  speechConfig.speechSynthesisOutputFormat = 8; //Audio24Khz160KBitRateMonoMp3

  player = new SpeechSDK.SpeakerAudioDestination();
  player.onAudioStart = function (_) {
    window.console.log("playback started");
    startCallback();
  };
  player.onAudioEnd = function (_) {
    window.console.log("playback finished");
    stopCallback();
  };

  const audioConfig = SpeechSDK.AudioConfig.fromSpeakerOutput(player);
  const synthesizer = new SpeechSDK.SpeechSynthesizer(
    speechConfig,
    audioConfig
  );

  // The event synthesizing signals that a synthesized audio chunk is received.
  // You will receive one or more synthesizing events as a speech phrase is synthesized.
  // You can use this callback to streaming receive the synthesized audio.
  synthesizer.synthesizing = function (s, e) {
    window.console.log(e);
    // eventsDiv.innerHTML +=
    //   "(synthesizing) Reason: " +
    //   SpeechSDK.ResultReason[e.result.reason] +
    //   "Audio chunk length: " +
    //   e.result.audioData.byteLength +
    //   "\r\n";
  };

  // The synthesis started event signals that the synthesis is started.
  synthesizer.synthesisStarted = function (s, e) {
    window.console.log(e);
    // eventsDiv.innerHTML += "(synthesis started)" + "\r\n";
    // pauseButton.disabled = false;
  };

  // The event synthesis completed signals that the synthesis is completed.
  synthesizer.synthesisCompleted = function (s, e) {
    console.log(e);
    // eventsDiv.innerHTML +=
    //   "(synthesized)  Reason: " +
    //   SpeechSDK.ResultReason[e.result.reason] +
    //   " Audio length: " +
    //   e.result.audioData.byteLength +
    //   "\r\n";
  };

  // The event signals that the service has stopped processing speech.
  // This can happen when an error is encountered.
  synthesizer.SynthesisCanceled = function (s, e) {
    const cancellationDetails = SpeechSDK.CancellationDetails.fromResult(
      e.result
    );
    let str =
      "(cancel) Reason: " +
      SpeechSDK.CancellationReason[cancellationDetails.reason];
    if (cancellationDetails.reason === SpeechSDK.CancellationReason.Error) {
      str += ": " + e.result.errorDetails;
    }
    window.console.log(e);
    // eventsDiv.innerHTML += str + "\r\n";
    // startSynthesisAsyncButton.disabled = false;
    // downloadButton.disabled = false;
    // pauseButton.disabled = true;
    // resumeButton.disabled = true;
  };

  // This event signals that word boundary is received. This indicates the audio boundary of each word.
  // The unit of e.audioOffset is tick (1 tick = 100 nanoseconds), divide by 10,000 to convert to milliseconds.
  synthesizer.wordBoundary = function (s, e) {
    window.console.log(e);
    // eventsDiv.innerHTML +=
    //   "(WordBoundary), Text: " +
    //   e.text +
    //   ", Audio offset: " +
    //   e.audioOffset / 10000 +
    //   "ms." +
    //   "\r\n";
    // wordBoundaryList.push(e);
  };

  synthesizer.visemeReceived = function (s, e) {
    window.console.log(e);
    // eventsDiv.innerHTML +=
    //   "(Viseme), Audio offset: " +
    //   e.audioOffset / 10000 +
    //   "ms. Viseme ID: " +
    //   e.visemeId +
    //   "\n";
    // talkingHeadDiv.innerHTML = e.animation.replaceAll(
    //   'begin="0.5s"',
    //   'begin="indefinite"'
    // );
    // $("svg").width("500px").height("500px");
  };

  synthesizer.bookmarkReached = function (s, e) {
    window.console.log(e);
    // eventsDiv.innerHTML +=
    //   "(Bookmark reached), Audio offset: " +
    //   e.audioOffset / 10000 +
    //   "ms. Bookmark text: " +
    //   e.text +
    //   "\n";
  };

  const complete_cb = function (result: any) {
    // if (result.reason === SpeechSDK.ResultReason.SynthesizingAudioCompleted) {
    //   resultsDiv.innerHTML += "synthesis finished";
    // } else if (result.reason === SpeechSDK.ResultReason.Canceled) {
    //   resultsDiv.innerHTML +=
    //     "synthesis failed. Error detail: " + result.errorDetails;
    // }
    window.console.log(result);
    synthesizer.close();
    //synthesizer = undefined;
  };
  const err_cb = function (err: any) {
    // startSynthesisAsyncButton.disabled = false;
    // downloadButton.disabled = false;
    // phraseDiv.innerHTML += err;
    window.console.log(err);
    synthesizer.close();
    //synthesizer = undefined;
  };

  //   if (!synthesisText.value) {
  //     alert("Please enter synthesis content.");
  //     return;
  //   }

  //   startSynthesisAsyncButton.disabled = true;
  //   downloadButton.disabled = true;

  //   if (isSsml.checked) {
  //     synthesizer.speakSsmlAsync(synthesisText.value, complete_cb, err_cb);
  //   } else {
  synthesizer.speakTextAsync(text, complete_cb, err_cb);
  //}
}
