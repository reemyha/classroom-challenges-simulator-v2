// WebSpeechRecognition.jslib
// Place this file in: Assets/Plugins/WebGL/WebSpeechRecognition.jslib
//
// This plugin connects Unity WebGL to the browser's Web Speech API
// Completely FREE - works in Chrome, Edge, Safari

var WebSpeechPlugin = {
    // Global variables
    $webSpeechState: {
        recognition: null,
        isRecognizing: false,
        gameObjectName: "WebSpeechRecognition", // Name of Unity GameObject to send callbacks to
        lastTranscript: "",
        lastError: ""
    },

    /**
     * Initialize Web Speech Recognition
     * @param {string} language - Language code (e.g., "en-US")
     * @param {boolean} continuous - Whether to keep listening after getting result
     */
    InitWebSpeechRecognition: function(language, continuous) {
        var lang = UTF8ToString(language);
        
        console.log("[WebSpeech] Initializing with language:", lang);

        // Check browser support
        var SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        
        if (!SpeechRecognition) {
            console.error("[WebSpeech] Browser does not support Web Speech API");
            console.error("[WebSpeech] Try using Chrome, Edge, or Safari");
            
            // Send error to Unity
            webSpeechState.lastError = "Browser not supported. Use Chrome, Edge, or Safari.";
            SendMessage(webSpeechState.gameObjectName, 'OnWebSpeechError', webSpeechState.lastError);
            return;
        }

        // Create recognition instance
        webSpeechState.recognition = new SpeechRecognition();
        
        // Configure recognition
        webSpeechState.recognition.continuous = continuous;
        webSpeechState.recognition.interimResults = false;
        webSpeechState.recognition.lang = lang;
        webSpeechState.recognition.maxAlternatives = 1;

        // Set up event handlers
        webSpeechState.recognition.onstart = function() {
            console.log("[WebSpeech] Recognition started");
            webSpeechState.isRecognizing = true;
            SendMessage(webSpeechState.gameObjectName, 'OnWebSpeechStart', '');
        };

        webSpeechState.recognition.onresult = function(event) {
            console.log("[WebSpeech] Result received");
            
            if (event.results.length > 0) {
                var result = event.results[event.results.length - 1];
                
                if (result.isFinal) {
                    var transcript = result[0].transcript;
                    var confidence = result[0].confidence;
                    
                    console.log("[WebSpeech] Transcript:", transcript);
                    console.log("[WebSpeech] Confidence:", confidence);
                    
                    webSpeechState.lastTranscript = transcript;
                    
                    // Send to Unity
                    SendMessage(webSpeechState.gameObjectName, 'OnWebSpeechResult', transcript);
                }
            }
        };

        webSpeechState.recognition.onerror = function(event) {
            console.error("[WebSpeech] Error:", event.error);
            
            var errorMsg = event.error;
            
            // Provide helpful error messages
            if (event.error === 'no-speech') {
                errorMsg = "No speech detected. Please try again.";
            } else if (event.error === 'audio-capture') {
                errorMsg = "No microphone found. Please check your microphone.";
            } else if (event.error === 'not-allowed') {
                errorMsg = "Microphone permission denied. Please allow microphone access.";
            } else if (event.error === 'network') {
                errorMsg = "Network error. Please check your internet connection.";
            }
            
            webSpeechState.lastError = errorMsg;
            webSpeechState.isRecognizing = false;
            
            // Send to Unity
            SendMessage(webSpeechState.gameObjectName, 'OnWebSpeechError', errorMsg);
        };

        webSpeechState.recognition.onend = function() {
            console.log("[WebSpeech] Recognition ended");
            webSpeechState.isRecognizing = false;
            SendMessage(webSpeechState.gameObjectName, 'OnWebSpeechEnd', '');
        };

        console.log("[WebSpeech] Initialization complete");
    },

    /**
     * Start speech recognition
     */
    StartWebSpeechRecognition: function() {
        if (!webSpeechState.recognition) {
            console.error("[WebSpeech] Recognition not initialized. Call InitWebSpeechRecognition first.");
            return;
        }

        if (webSpeechState.isRecognizing) {
            console.warn("[WebSpeech] Already recognizing");
            return;
        }

        try {
            console.log("[WebSpeech] Starting recognition...");
            webSpeechState.recognition.start();
        } catch (e) {
            console.error("[WebSpeech] Failed to start:", e);
            webSpeechState.lastError = e.toString();
            SendMessage(webSpeechState.gameObjectName, 'OnWebSpeechError', webSpeechState.lastError);
        }
    },

    /**
     * Stop speech recognition
     */
    StopWebSpeechRecognition: function() {
        if (!webSpeechState.recognition) {
            console.error("[WebSpeech] Recognition not initialized");
            return;
        }

        if (!webSpeechState.isRecognizing) {
            console.warn("[WebSpeech] Not currently recognizing");
            return;
        }

        try {
            console.log("[WebSpeech] Stopping recognition...");
            webSpeechState.recognition.stop();
        } catch (e) {
            console.error("[WebSpeech] Failed to stop:", e);
        }
    },

    /**
     * Check if currently recognizing
     */
    IsWebSpeechRecognizing: function() {
        return webSpeechState.isRecognizing;
    },

    /**
     * Get last transcript (for debugging)
     */
    GetLastWebSpeechTranscript: function() {
        var bufferSize = lengthBytesUTF8(webSpeechState.lastTranscript) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(webSpeechState.lastTranscript, buffer, bufferSize);
        return buffer;
    },

    /**
     * Get last error (for debugging)
     */
    GetLastWebSpeechError: function() {
        var bufferSize = lengthBytesUTF8(webSpeechState.lastError) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(webSpeechState.lastError, buffer, bufferSize);
        return buffer;
    }
};

// Register the plugin
autoAddDeps(WebSpeechPlugin, '$webSpeechState');
mergeInto(LibraryManager.library, WebSpeechPlugin);
