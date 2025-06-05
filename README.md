# HoloLingo
This project is an immersive Mixed Reality (MR) educational game that leverages Meta Passthrough Camera API, AI-powered object detection, and Google Text-to-Speech and Speech-to-Text technologies to teach new languages in a fun and interactive way. The app challenges players to identify real-world objects in their environment and name them in a target language, enhancing their language skills in an engaging and practical way.

# Overview
This Mixed Reality (MR) language learning game uses real-time object detection, AI-powered identification, and interactive holographic AR labels to teach users new words in a variety of languages. Players point to real-world objects, and the AI asks them to say the name of the object in the target language. If they get it right, they are rewarded with points and feedback on their pronunciation, helping them learn and practice languages more effectively. The system uses Meta Passthrough Camera API for detecting objects in the real-world environment, OpenAI for object recognition, and Google Cloud's Speech-to-Text and Text-to-Speech APIs for recognizing speech and providing pronunciation feedback.

# Features
**Core Features:**
1. **Real-time Object Detection via Passthrough Camera:** Uses Meta's Passthrough API to detect objects in the player’s environment.
2. **AI-powered Object Identification:** The app identifies objects in the environment and prompts the user to say their names in a target language.
3. **Interactive Voice Prompts:** Players are asked to say the name of an object in the target language, e.g., “Say 'chair' in Spanish.”
4. **Pronunciation Feedback:** The app listens to the user’s response, compares it to the correct pronunciation, and provides positive or corrective feedback.

# Tech Stack
1. **Meta Quest SDK:** Used for Passthrough camera functionality and immersive AR integration.

2. **AI Object Recognition API:** Utilized to identify and label objects in the player's environment.

3. **Speech Recognition & Pronunciation Feedback (Google Speech-to-Text & Text-to-Speech):** Used for capturing user input and providing pronunciation feedback.

4. **Unity Game Engine:** The core game engine used for building and deploying the application.
