import sys
import time

from google.cloud import automl_v1beta1
from google.cloud.automl_v1beta1.proto import service_pb2

def get_prediction(content, project_id, model_id):
  prediction_client = automl_v1beta1.PredictionServiceClient()

  name = 'projects/{}/locations/us-central1/models/{}'.format(project_id, model_id)
  payload = {'image': {'image_bytes': content }}
  params = {}
  request = prediction_client.predict(name, payload, params)
  return request  # waits till request is returned

if __name__ == '__main__':
  start_time = time.time() 
  file_path = sys.argv[1]
  project_id = 'no-voice'
  model_id = 'ICN6380204732493198083'
  
  with open(file_path, 'rb') as ff:
    content = ff.read()
  result = get_prediction(content, project_id,  model_id)

  print ()
  cat = result.payload[0].display_name
  acc = result.payload[0].classification.score
  print(cat)
  print((int)(round(acc,1)*100))
  print(round((time.time() - start_time), 1))