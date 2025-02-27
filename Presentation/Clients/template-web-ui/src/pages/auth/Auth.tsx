import React, { useState } from "react";
import { Container } from "semantic-ui-react";
import { motion } from "framer-motion";
import Login from "./components/Login";
import Register from "./components/Register";

const Auth: React.FC = () => {
  const [isLogin, setIsLogin] = useState(true);

  const toggleForm = () => {
    setIsLogin(!isLogin);
  };

  return (
    <Container style={{ width: "400px", marginTop: "100px" }}>
      <motion.div
        key={isLogin ? "login" : "register"}
        initial={{ opacity: 0, x: -50 }}
        animate={{ opacity: 1, x: 0 }}
        exit={{ opacity: 0, x: 50 }}
        transition={{ duration: 0.5 }}
      >
        {isLogin ? (
          <Login toggleForm={toggleForm} />
        ) : (
          <Register toggleForm={toggleForm} />
        )}
      </motion.div>
    </Container>
  );
};

export default Auth;
