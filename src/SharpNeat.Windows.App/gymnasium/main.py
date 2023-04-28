import struct
import time
import traceback

import gymnasium as gym
from gymnasium import spaces
from gymnasium.wrappers import ClipAction
import win32pipe
import win32file
from argparse import ArgumentParser
import numpy as np
import logging


logging.basicConfig(filename='debug.log', encoding='utf-8', level=logging.FATAL)
logging.debug("start")

parser = ArgumentParser()
parser.add_argument("-uuid", dest="uuid")
parser.add_argument("-render", dest="render")
args = parser.parse_args()
render = args.render == "True"

pipe = win32pipe.CreateNamedPipe("\\\\.\\pipe\\gymnasium_pipe_" + args.uuid,
                                 win32pipe.PIPE_ACCESS_DUPLEX,
                                 win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE | win32pipe.PIPE_WAIT,
                                 1, 1024, 1024, 0, None)
logging.debug("Connecting pipe...")
win32pipe.ConnectNamedPipe(pipe, None)
logging.debug("Pipe connected")

# env = gym.make("BipedalWalker-v3", hardcore=False, render_mode="human" if render else None)
# env = gym.make("LunarLander-v2", render_mode="human" if render else None)
try:
    # env = gym.make("LunarLander-v2", enable_wind=True, render_mode="human" if render else None)
    env = gym.make("BipedalWalker-v3", hardcore=True, render_mode="human" if render else None)
    env = ClipAction(env)

    logging.debug("Environment created")
    logging.debug("Environment action size: %s", str(env.action_space.shape[0]))
    logging.debug("Environment action type: %s", str(env.action_space.dtype))
    logging.debug("Environment action type size: %s", str(env.action_space.dtype.itemsize))
    logging.debug("Environment action type char: %s", str(env.action_space.dtype.char))
except Exception as e:
    logging.error(e)


def run_episode():
    observation, info = env.reset()

    logging.debug("Initial observation:", observation)
    send_observation(observation, 0, False)
    logging.debug("Initial observation sent")

    total_reward = 0
    total_timesteps = 0

    while 1:
        logging.debug("Starting step")
        a = read_action(env.action_space)
        logging.debug("Action read:", a)

        total_timesteps += 1

        observation, reward, terminated, truncated, info = env.step(a)
        logging.debug(observation)

        done = terminated or truncated

        # if reward != 0:
        #     print("reward %0.3f" % reward)

        total_reward += reward

        masked_done = done

        # if render:
        #     masked_done = False

        send_observation(observation, float(total_reward), masked_done)
        logging.debug("Observation sent")

        if render:
            env.render()
            time.sleep(0.02)

        if done:
            logging.debug("Terminated")
            if not render:
                # pipe.close()
                env.close()
                break
            else:
                env.close()
                # env.reset()
            # print(reward)
            # input("Done")
    # print("timesteps %i reward %0.2f" % (total_timesteps, total_reward))


def send_observation(observation: np.array, reward: float, done: bool):
    win32file.WriteFile(pipe, bytes(observation.astype(float)) + bytes(np.array([reward]).astype(float)) + bytes(np.array([int(done)])))


def read_action(space: spaces.Space):
    is_discrete = len(space.shape) == 0
    count = 1 if is_discrete else space.shape[0]
    type_char = 'i' if is_discrete else space.dtype.char
    item_size = 4 if is_discrete else space.dtype.itemsize
    result, action_struct = win32file.ReadFile(pipe, item_size * count)
    action_got = struct.unpack(count * type_char, action_struct)
    return action_got[0] if is_discrete else action_got


def read_int_action():
    result, action_struct = win32file.ReadFile(pipe, 4)
    action_got = struct.unpack('i', action_struct)[0]
    return action_got


def read_float_action(count):
    result, action_struct = win32file.ReadFile(pipe, 8 * count)
    action_got = struct.unpack('dddd', action_struct)
    return action_got


try:
    run_episode()
except Exception as e:
    logging.error(str(e))
    logging.error(traceback.format_exc())
