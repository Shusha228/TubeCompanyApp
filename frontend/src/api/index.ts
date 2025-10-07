const BASE_URL: string | undefined = import.meta.env.VITE_API_URL;

const SAFE_BASE_URL = BASE_URL?.endsWith("/")
  ? BASE_URL.slice(0, BASE_URL.length - 1)
  : BASE_URL;

export const getURL = (url: string): string => {
  return SAFE_BASE_URL + "/" + url;
};
