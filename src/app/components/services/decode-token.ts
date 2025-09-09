import { jwtDecode } from 'jwt-decode';

export function getUserIdFromToken(token: string): string | null {
  try {
    const decoded: any = jwtDecode(token);
    const userId =
      decoded[
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
      ];

    return userId || null;
  } catch (error) {
    console.error('Error decoding token', error);
    return null;
  }
}
