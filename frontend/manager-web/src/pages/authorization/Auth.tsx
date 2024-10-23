import styled from "styled-components";
import { useForm } from "react-hook-form";

import useToggle from "hooks/useToggle";
import Input from "components/input/Input";
import mainLogo from "images/main_logo.png";
import { useSelector } from "react-redux";
import { authErrorSelector } from "./authSelector";
import ErrorMessage from "components/ruleEngine/components/rules/ErrorMessage";
import { useContext } from "react";
import { AuthContext } from "data/user/AuthContext";
export interface ILoginForm {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export default function AuthPage() {
  const authError = useSelector(authErrorSelector);
  const { login } = useContext(AuthContext);
  const [isPasswordShown, setIsPasswordShown] = useToggle();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ILoginForm>();

  const loginHandler = (data: ILoginForm) => {
    login(data);
  };

  return (
    <AuthWrapper>
      <AuthForm>
        <LogoContainer>
          <Logo src={mainLogo} alt="aorta" />
          <EnvContainer>
            <Side>Manager</Side>
            {/*<Env env={process.env.REACT_APP_ENV}>{process.env.REACT_APP_ENV}</Env>*/}
          </EnvContainer>
        </LogoContainer>
        <AuthType>Login</AuthType>
        <form onSubmit={handleSubmit(loginHandler)}>
          <Label>
            <InputWrapper>
              <Input
                inputIcon={<i className="icon-user" />}
                {...register("email", {
                  required: true,
                })}
                autoComplete="on"
                type="email"
                placeholder="Email"
                name="email"
              />
            </InputWrapper>
            <InputWrapper>
              <ErrorMessage>
                {errors.email && "Email is required and should be in a valid format"}
              </ErrorMessage>
            </InputWrapper>
          </Label>
          <Label>
            <InputWrapper>
              <Input
                {...register("password", {
                  required: true,
                })}
                inputIcon={<i className="icon-lock" />}
                autoComplete="on"
                type={isPasswordShown ? "text" : "password"}
                placeholder="Password"
                name="password"
              />
              <i className="icon-eye" onClick={setIsPasswordShown} />
            </InputWrapper>
            <InputWrapper>
              <ErrorMessage>{errors.password && "Password is requeired field"}</ErrorMessage>
            </InputWrapper>
          </Label>
          <Checkbox>
            <input type="checkbox" {...register("rememberMe")} />
            Remember me
          </Checkbox>
          <StyledButton>Login</StyledButton>
          <InputWrapper>
            <ErrorMessage>{authError}</ErrorMessage>
          </InputWrapper>
        </form>
      </AuthForm>
    </AuthWrapper>
  );
}

const AuthWrapper = styled.div`
  width: 100vw;
  height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background-color: ${({ theme }) => theme.colors.bg.primary};
`;

const AuthForm = styled.div`
  background-color: ${({ theme }) => theme.colors.bg.secondary};
  padding: 75px 95px;
  box-sizing: border-box;
  box-shadow: 0 4px 4px rgba(0, 0, 0, 0.16);
  border-radius: 4px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  h1 {
    display: flex;
    justify-content: center;
    align-items: center;
    flex: 3 1 auto;
  }
  form {
    flex: 3 1 auto;
    height: max-content;
    width: max-content;
  }
  button {
    width: 100%;
    height: 36px;
    background-color: ${({ theme }) => theme.colors.btn.secondary};
    color: ${({ theme }) => theme.colors.bg.ternary};
    border: none;
    text-transform: uppercase;
  }
`;

const LogoContainer = styled.div`
  margin-bottom: 40px;
  position: relative;
`;

const Logo = styled.img``;

const EnvContainer = styled.div`
  height: 100%;
  position: absolute;
  top: 0;
  right: 0;
  transform: translateX(100%);
  color: ${({ theme }) => theme.colors.btn.secondary};
`;

const Side = styled.div`
  font-family: "Inter medium", serif;
  font-size: 12px;
  line-height: 24px;
  padding: 1px 10px 0;
  box-sizing: border-box;
`;

const Env = styled.div<{ env?: string }>`
  position: absolute;
  right: -10px;
  top: 0;
  background-color: ${props =>
    props.env === "dev" ? "green" : props.env === "qa" ? "yellow" : "red"};
  line-height: 24px;
  padding: 1px 10px 0;
  border-radius: 9px;
  text-transform: uppercase;
  font-family: "Inter medium", serif;
  font-size: 12px;
  color: white;
  border: 1px solid ${({ theme }) => theme.colors.border.secondary};
  box-sizing: border-box;
  transform: translateX(100%);
`;

const AuthType = styled.h3`
  text-transform: uppercase;
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 24px;
  width: 100%;
  font-size: 14px;
  line-height: 17px;
  padding: 12px 0;
  font-family: "Inter medium", serif;
  color: ${({ theme }) => theme.colors.btn.secondary};
  border-bottom: 3px solid ${({ theme }) => theme.colors.border.secondary};
`;

const Label = styled.label`
  display: flex;
  flex-direction: column;
  margin-bottom: 40px;
  ${({ theme }) => theme.typography.subtitle2}
`;

const InputWrapper = styled.div`
  position: relative;
  display: flex;
  flex-direction: column;
  justify-content: center;
  width: 100%;
  ${({ theme }) => theme.typography.body1}

  i {
    position: absolute;
  }
  .icon-eye {
    font-size: 2rem;
    right: 0.4rem;
    :before {
      color: ${({ theme }) => theme.colors.btn.secondary};
    }
  }
  .icon-user,
  .icon-lock {
    font-size: 1.5rem;
    top: 0.4rem;
    left: 0.4rem;
    color: black;
    opacity: 0.6;
  }
`;

const Checkbox = styled.label`
  display: flex;
  align-items: center;
  cursor: pointer;
  margin-bottom: 45px;
  color: black;
  opacity: 0.6;
  ${({ theme }) => theme.typography.smallText1}
  line-height: 24px;
`;

const StyledButton = styled.button``;
