from unittest import TestCase
from src.app.minimalhand.absolute_to_relative_converter import CoordenateConverter


# class TestCoordenateConverter(TestCase):
#     def test_convert_to_relative(self):
import cv2
import mediapipe as mp

mp_drawing = mp.solutions.drawing_utils
mp_hands = mp.solutions.hands

converter = CoordenateConverter()

# For webcam input:
hands = mp_hands.Hands(min_detection_confidence=0.5, min_tracking_confidence=0.5)
cap = cv2.VideoCapture(0)
while cap.isOpened():
    success, image = cap.read()
    if not success:
        print("Ignoring empty camera frame.")
        # If loading a video, use 'break' instead of 'continue'.
        continue

    # Flip the image horizontally for a later selfie-view display, and convert
    # the BGR image to RGB.
    image = cv2.cvtColor(cv2.flip(image, 1), cv2.COLOR_BGR2RGB)
    # To improve performance, optionally mark the image as not writeable to
    # pass by reference.
    image.flags.writeable = False
    results = hands.process(image)

    # Draw the hand annotations on the image.
    image.flags.writeable = True
    image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
    if results.multi_hand_landmarks is not None:
        coordenates = results.multi_hand_landmarks

        relative_coordenates = converter.convert_to_relative(absolute_coordenates=coordenates)

        new_coordenates = converter.convert_to_absolute(relative_coordenates)

        print(coordenates)
        print(new_coordenates)
        if new_coordenates:
            for hand_landmarks in new_coordenates:
                mp_drawing.draw_landmarks(image, hand_landmarks, mp_hands.HAND_CONNECTIONS)
    cv2.imshow('MediaPipe Hands', image)
    if cv2.waitKey(10) & 0xFF == 27:
        break
hands.close()
cap.release()

print(coordenates)
print(relative_coordenates)
print(absolute2_coordenates)

