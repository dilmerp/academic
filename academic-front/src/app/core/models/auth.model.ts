export interface AuthResponseDto {
  token: string;
  nombreCompleto: string;
  login: string;
  requiereCambioClave: boolean;
}

export interface ActualizarClaveRequestDto {
  login: string;
  nuevaClave: string;
}
