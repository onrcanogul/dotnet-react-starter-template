export interface Token {
  accessToken: string;
  refreshToken: string;
  expiration: Date;
}

export interface DecodedToken {
  userId: string;
  name: string;
  exp: number; // Token'ın geçerlilik süresi
}
