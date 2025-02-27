interface ServiceResponse<T> extends Response {
  data: T;
  statusCode: number;
  isSuccessful: boolean;
  errors: string[];
}

interface Response {
  statusCode: number;
  isSuccessful: boolean;
  errors: string[];
}

export interface NoContent {}

export default ServiceResponse;
