import { toast } from "react-toastify";

const toastrConfig = {
  position: "top-right" as const,
  autoClose: 3000,
  hideProgressBar: false,
  closeOnClick: true,
  pauseOnHover: true,
  draggable: true,
  progress: undefined,
};

const success = (message: string) => {
  toast.success(message, toastrConfig);
};

const warning = (message: string) => {
  toast.warn(message, toastrConfig);
};

const info = (message: string) => {
  toast.info(message, toastrConfig);
};

const error = (message: string) => {
  toast.error(message, toastrConfig);
};

const ToastrService = {
  success,
  warning,
  info,
  error,
};

export default ToastrService;
