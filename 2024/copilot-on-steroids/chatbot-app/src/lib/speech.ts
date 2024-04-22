import * as SpeechSDK from "microsoft-cognitiveservices-speech-sdk";

const key = "the cognitiveservices key - but in real app you should get a token from the backend to not expose the key";
const region = "westeurope";
let reco: any;


export function doRecognizeOnceAsync(
  audioFile: any,
  callback: any,
  debug: boolean
) {
  var audioConfig = getAudioConfig(audioFile, debug);
  var speechConfig = getSpeechConfig(SpeechSDK.SpeechConfig);
  if (!audioConfig || !speechConfig) return;

  // Create the SpeechRecognizer and set up common event handlers and PhraseList data
  reco = new SpeechSDK.SpeechRecognizer(speechConfig, audioConfig);
  // var autoDetectSourceLanguageConfig =
  //   SpeechSDK.AutoDetectSourceLanguageConfig.fromLanguages([
  //     "en-US",
  //     "fr-FR",
  //     "it-IT",
  //     "es-ES",
  //   ]);

  // reco = SpeechSDK.SpeechRecognizer.FromConfig(
  //   speechConfig,
  //   autoDetectSourceLanguageConfig,
  //   audioConfig
  // );
  applyCommonConfigurationTo(reco);

  // Note: in this scenario sample, the 'recognized' event is not being set to instead demonstrate
  // continuation on the 'recognizeOnceAsync' call. 'recognized' can be set in much the same way as
  // 'recognizing' if an event-driven approach is preferable.
  reco.recognized = undefined;

  // Note: this scenario sample demonstrates result handling via continuation on the recognizeOnceAsync call.
  // The 'recognized' event handler can be used in a similar fashion.
  reco.recognizeOnceAsync(
    function (successfulResult: any) {
      onRecognizedResult(successfulResult);
      callback(successfulResult);
    },
    function (err: string) {
      window.console.log(err);
    }
  );
}

function onRecognizedResult(result: any) {
  console.log(result);
}

function getAudioConfig(audioFile: any, debug: boolean) {
  // If an audio file was specified, use it. Otherwise, use the microphone.
  // Depending on browser security settings, the user may be prompted to allow microphone use. Using
  // continuous recognition allows multiple phrases to be recognized from a single use authorization.
  // if (audioFile) {
  //   return SpeechSDK.AudioConfig.fromWavFileInput(audioFile);
  // } else if (inputSourceFileRadio.checked) {
  //   alert(
  //     "Please choose a file when selecting file input as your audio source."
  //   );
  //   return;
  // } else if (microphoneSources.value) {
  //   return SpeechSDK.AudioConfig.fromMicrophoneInput(microphoneSources.value);
  // } else {
  if (debug) {
    return SpeechSDK.AudioConfig.fromWavFileInput(audioFile);
  }

  return SpeechSDK.AudioConfig.fromDefaultMicrophoneInput();

  // }
}

function getSpeechConfig(sdkConfigType: typeof SpeechSDK.SpeechConfig): any {
  let speechConfig;
  // if (authorizationToken) {
  //   speechConfig = sdkConfigType.fromAuthorizationToken(
  //     authorizationToken,
  //     regionOptions.value
  //   );
  // } else if (!key.value) {
  //   alert("Please enter your Cognitive Services Speech subscription key!");
  //   return undefined;
  // } else {
  speechConfig = sdkConfigType.fromSubscription(key, region);

  // Setting the result output format to Detailed will request that the underlying
  // result JSON include alternates, confidence scores, lexical forms, and other
  // advanced information.
  // if (useDetailedResults && sdkConfigType != SpeechSDK.SpeechConfig) {
  //   window.console.log(
  //     "Detailed results are not supported for this scenario.\r\n"
  //   );
  //   document.getElementById("formatSimpleRadio").click();
  // } else if (useDetailedResults) {
  speechConfig.outputFormat = SpeechSDK.OutputFormat.Simple;
  // }

  // Defines the language(s) that speech should be translated to.
  // Multiple languages can be specified for text translation and will be returned in a map.
  // if (sdkConfigType == SpeechSDK.SpeechTranslationConfig) {
  //   speechConfig.addTargetLanguage(
  //     languageTargetOptions.value.split("(")[1].substring(0, 5)
  //   );
  // }

  speechConfig.speechRecognitionLanguage = "en-GB"; //languageOptions.value;
  return speechConfig;
}

function applyCommonConfigurationTo(recognizer: any) {
  // The 'recognizing' event signals that an intermediate recognition result is received.
  // Intermediate results arrive while audio is being processed and represent the current "best guess" about
  // what's been spoken so far.
  recognizer.recognizing = onRecognizing;

  // The 'recognized' event signals that a finalized recognition result has been received. These results are
  // formed across complete utterance audio (with either silence or eof at the end) and will include
  // punctuation, capitalization, and potentially other extra details.
  //
  // * In the case of continuous scenarios, these final results will be generated after each segment of audio
  //   with sufficient silence at the end.
  // * In the case of intent scenarios, only these final results will contain intent JSON data.
  // * Single-shot scenarios can also use a continuation on recognizeOnceAsync calls to handle this without
  //   event registration.
  recognizer.recognized = onRecognized;

  // The 'canceled' event signals that the service has stopped processing speech.
  // https://docs.microsoft.com/javascript/api/microsoft-cognitiveservices-speech-sdk/speechrecognitioncanceledeventargs?view=azure-node-latest
  // This can happen for two broad classes of reasons:
  // 1. An error was encountered.
  //    In this case, the .errorDetails property will contain a textual representation of the error.
  // 2. No additional audio is available.
  //    This is caused by the input stream being closed or reaching the end of an audio file.
  recognizer.canceled = onCanceled;

  // The 'sessionStarted' event signals that audio has begun flowing and an interaction with the service has
  // started.
  recognizer.sessionStarted = onSessionStarted;

  // The 'sessionStopped' event signals that the current interaction with the speech service has ended and
  // audio has stopped flowing.
  recognizer.sessionStopped = onSessionStopped;

  // PhraseListGrammar allows for the customization of recognizer vocabulary.
  // The semicolon-delimited list of words or phrases will be treated as additional, more likely components
  // of recognition results when applied to the recognizer.
  //
  // See https://docs.microsoft.com/azure/cognitive-services/speech-service/get-started-speech-to-text#improve-recognition-accuracy
  // if (phrases.value) {
  //   var phraseListGrammar =
  //     SpeechSDK.PhraseListGrammar.fromRecognizer(recognizer);
  //   phraseListGrammar.addPhrases(phrases.value.split(";"));
  // }
}

function onRecognizing(sender: any, recognitionEventArgs: { result: any }) {
  var result = recognitionEventArgs.result;
  // statusDiv.innerHTML +=
  //   `(recognizing) Reason: ${SpeechSDK.ResultReason[result.reason]}` +
  //   ` Text: ${result.text}\r\n`;
  // // Update the hypothesis line in the phrase/result view (only have one)
  // phraseDiv.innerHTML =
  //   phraseDiv.innerHTML.replace(/(.*)(^|[\r\n]+).*\[\.\.\.\][\r\n]+/, "$1$2") +
  //   `${result.text} [...]\r\n`;
  // phraseDiv.scrollTop = phraseDiv.scrollHeight;
}

function onRecognized(sender: any, recognitionEventArgs: { result: any }) {
  var result = recognitionEventArgs.result;
  onRecognizedResult(recognitionEventArgs.result);
}

function onCanceled(sender: any, cancellationEventArgs: any) {
  window.console.log(cancellationEventArgs);

  // statusDiv.innerHTML +=
  //   "(cancel) Reason: " +
  //   SpeechSDK.CancellationReason[cancellationEventArgs.reason];
  // if (cancellationEventArgs.reason === SpeechSDK.CancellationReason.Error) {
  //   statusDiv.innerHTML += ": " + cancellationEventArgs.errorDetails;
  // }
  // statusDiv.innerHTML += "\r\n";
}

function onSessionStarted(sender: any, sessionEventArgs: any) {
  // statusDiv.innerHTML += `(sessionStarted) SessionId: ${sessionEventArgs.sessionId}\r\n`;
  // for (const thingToDisableDuringSession of thingsToDisableDuringSession) {
  //   thingToDisableDuringSession.disabled = true;
  // }
  // scenarioStartButton.disabled = true;
  // scenarioStopButton.disabled = false;
}

function onSessionStopped(sender: any, sessionEventArgs: any) {
  // statusDiv.innerHTML += `(sessionStopped) SessionId: ${sessionEventArgs.sessionId}\r\n`;
  // if (
  //   scenarioSelection.value == "pronunciationAssessmentContinuous" ||
  //   scenarioSelection.value == "pronunciationAssessmentContinuousStream"
  // ) {
  //   calculateOverallPronunciationScore();
  // }
  // for (const thingToDisableDuringSession of thingsToDisableDuringSession) {
  //   thingToDisableDuringSession.disabled = false;
  // }
  // scenarioStartButton.disabled = false;
  // scenarioStopButton.disabled = true;
}
