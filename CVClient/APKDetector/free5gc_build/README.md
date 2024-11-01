# Buildup Android Application for Free5GC Hand Detector

- Build Android App 

  ```shell
  bazel build -c opt --config=android_arm64 mediapipe/projects/android/src/java/com/google/mediapipe/apps/project:free5gc_handdetector
  ```

- reference
  - [bazel](https://bazel.build/start?hl=zh-tw)

- Package Info

  ```
  android_sdk_repository(name = "androidsdk")
  android_ndk_repository(name = "androidndk", api_level=21)
  ```

  
