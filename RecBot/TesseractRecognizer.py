from craft_text_detector import Craft
import pytesseract
import httplib2
import PIL
import uuid
import os
import sys

pytesseract.pytesseract.tesseract_cmd = r"full path to the exe file"
pytesseract.pytesseract.tesseract_cmd = r"C:\Program Files (x86)\Tesseract-OCR\tesseract.exe"

class Recognizer:
    def __init__(self, image_link, image_name):
        self.image_link = image_link
        self.image_name = image_name
        self.output_dir = fr'C:\Users\Public\Downloads\Recognizer\{self.image_name}'
        self.image_path = fr'{self.output_dir}\{self.image_name}.jpg'
        self.texts_path = fr'{self.output_dir}\texts.txt'

        os.mkdir(self.output_dir)
    
    def craft_detect(self):
        craft = Craft(output_dir=self.output_dir, crop_type='poly',cuda=False)
        prediction_result = craft.detect_text(self.image_path)

        craft.unload_craftnet_model()
        craft.unload_refinenet_model()

    def download_image(self):
        content = ''

        if self.image_link.find('http') == 0:
            h = httplib2.Http('.cache')
            responce, content = h.request(self.image_link)
        elif self.image_link.find('C:') == 0:
            f = open(fr'C:\Users\Public\Downloads\Recognizer\tmp\{self.image_name}.txt', 'rb')
            content = f.read()

        with open(self.image_path, 'wb') as out:
            out.write(content)

    def recognize(self):
        #crops_path = fr'{self.output_dir}\{self.image_name}_crops'
        images = [ self.image_path ]
        #images += [ fr'{crops_path}\{img}' for img in os.listdir(crops_path) ]

        texts = []

        for img in images:
            text = pytesseract.image_to_string(PIL.Image.open(img), lang='rus')
            texts.append(text)
            
        with open(self.texts_path, 'w', encoding='utf-8') as out:
            for text in texts:
                out.write(text)

image_link = sys.argv[1]
image_name = sys.argv[2]

recognizer = Recognizer(image_link, image_name)

recognizer.download_image()
#recognizer.craft_detect()
recognizer.recognize()